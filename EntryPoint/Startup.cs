using System;
using System.Data.Common;
using EntryPoint.Application.Behaviors;
using EntryPoint.Application.IntegrationEvents;
using EntryPoint.Application.IntegrationEvents.EventHandlers;
using EntryPoint.Domain.Aggregates.Order;
using EntryPoint.Infrastructure;
using EntryPoint.Infrastructure.Repositories;
using EventBus;
using EventBus.Abstractions;
using EventBus.RabbitMQ;
using IntegrationEventLog.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EntryPoint
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMediatR(typeof(Startup));

            services.AddEntityFrameworkNpgsql();

            services.AddDbContext<MicroserviceContext>(options =>

                options
                    .UseNpgsql("Host=localhost;Database=MicroserviceDb;Username=db_user;Password=db_pass",
                        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<IntegrationEventLogService>>();
                    return connection => new IntegrationEventLogService(connection, logger);
                });


            services
                .AddTransient<OrderStatusChangedIntegrationEventHandler>();

            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQConnection>();

                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();

                var subManager = sp.GetRequiredService<IEventBusSubscriptionManager>();

                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

                return new RabbitMQEventBus(connection, logger, serviceScopeFactory, subManager, "EntryPoint", 3);
            });

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();

                var factory = new ConnectionFactory();

                return new RabbitMQConnection(factory, logger, 3);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

using System;
using System.Data.Common;
using EventBus;
using EventBus.Abstractions;
using EventBus.RabbitMQ;
using IntegrationEventLog.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders.Application.Behaviors;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.EventHandlers;
using Orders.Application.Queries;
using Orders.Domain.Aggregates.Order;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;
using RabbitMQ.Client;

namespace Orders
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMediatR(typeof(Startup));

            services.AddEntityFrameworkNpgsql();

            var connectionString = _config.GetValue<string>("ConnectionString");

            services.AddDbContext<MicroserviceContext>(options =>

                options
                    .UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

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

            services.AddTransient<OrderStatusChangedIntegrationEventHandler>();

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

            services.AddScoped<IOrderQueries, OrderQueries>(_ => new OrderQueries(connectionString));

            services.AddCors();


            services.AddAuthentication().AddCookie("Cookies").AddOpenIdConnect(options =>
            {
                options.Authority = "http://localhost:3000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "EntryPoint";
            });
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

            app.UseCors(builder => builder.WithOrigins(_config.GetValue<string>("WebUrl")));


            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

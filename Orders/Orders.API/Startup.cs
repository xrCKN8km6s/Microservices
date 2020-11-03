using System.Collections.Generic;
using EventBus;
using EventBus.Redis;
using IdentityServer4.AccessTokenValidation;
using IntegrationEventLog;
using IntegrationEventLog.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Orders.API.Application.Behaviors;
using Orders.API.Application.IntegrationEvents;
using Orders.API.Application.IntegrationEvents.EventHandlers;
using Orders.API.Application.IntegrationEvents.Events;
using Orders.API.Application.Queries;
using Orders.Domain.Aggregates.Order;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;

namespace Orders.API
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
            services
                .AddControllers(options =>
                    options.Filters.Add(
                        new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError))
                )
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });

            AddAuthentication(services);
            AddAuthorization(services);
            AddEventBus(services);
            AddSwagger(services);

            services.AddMediatR(typeof(Startup));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

            var connectionString = _config.GetValue<string>("ConnectionString");

            AddDatabase(services, connectionString);

            services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            services.AddScoped<IOrderQueries, OrderQueries>(_ => new OrderQueries(connectionString));

            services.AddTransient<OrderStatusChangedIntegrationEventHandler>();
        }

        private static void AddDatabase(IServiceCollection services, string connectionString)
        {
            static void ConfigureNpgsql(NpgsqlDbContextOptionsBuilder npgsqlOptions) =>
                npgsqlOptions.EnableRetryOnFailure();

            services.AddDbContext<OrdersContext>(options => { options.UseNpgsql(connectionString, ConfigureNpgsql); });

            services.AddDbContext<IntegrationEventLogContext>((sp, options) =>
            {
                var context = sp.GetRequiredService<OrdersContext>();
                var dbConnection = context.Database.GetDbConnection();

                options.UseNpgsql(dbConnection, ConfigureNpgsql);
            });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = _config["identityUrlInternal"];
                    options.Audience = "orders";
                });
        }

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope("orders")
                    .Build();
            });
        }

        private void AddEventBus(IServiceCollection services)
        {
            services.AddEventBus(builder =>
            {
                builder
                    .UseInMemorySubscriptionManager()
                    .UseJsonNetSerializer()
                    .UseRedis(_config.GetSection("RedisEventBus").Get<RedisEventBusOptions>());
                    //.UseRabbitMQ(
                    //    _config.GetSection("RabbitMQ:Connection").Get<RabbitMQConnectionOptions>(),
                    //    _config.GetSection("RabbitMQ:EventBus").Get<RabbitMQEventBusOptions>()
                    //);
            });

            services.AddTransient<OrderStatusChangedIntegrationEventHandler>();
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddOpenApiDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "Orders API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new[] { "orders" }, new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flow = OpenApiOAuth2Flow.AccessCode,
                        AuthorizationUrl = $"{_config["identityUrl"]}/connect/authorize",
                        TokenUrl = $"{_config["identityUrl"]}/connect/token",
                        Scopes = new Dictionary<string, string>
                        {
                            {"orders", "Orders"}
                        }
                    })
                );

                document.OperationProcessors.Add(
                    new OperationSecurityScopeProcessor("oauth2"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "ordersswaggerui",
                    AppName = "Orders API",
                    UsePkceWithAuthorizationCodeGrant = true
                };
            });

            app.UseEndpoints(builder =>
            {
                builder
                    .MapControllers()
                    .RequireAuthorization();
            });

            var bus = app.ApplicationServices.GetRequiredService<IEventBus>();

            bus.Subscribe(sub =>
            {
                sub.Add<OrderStatusChangedIntegrationEvent, OrderStatusChangedIntegrationEventHandler>();
            });
        }
    }
}

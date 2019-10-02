using System.Collections.Generic;
using System.Linq;
using EventBus;
using EventBus.RabbitMQ;
using IdentityServer4.AccessTokenValidation;
using IntegrationEventLog;
using IntegrationEventLog.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Orders.API.Application.Behaviors;
using Orders.API.Application.IntegrationEvents;
using Orders.API.Application.IntegrationEvents.EventHandlers;
using Orders.API.Application.Queries;
using Orders.Domain.Aggregates.Order;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;
using RabbitMQ.Client;

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
                .AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(w => w.Value.ValidationState == ModelValidationState.Invalid)
                            .ToDictionary(k => k.Key, v => v.Value.Errors.Select(s => s.ErrorMessage));

                        var problemDetails = new ValidationErrorDetails(
                            context.HttpContext.TraceIdentifier,
                            errors
                        );

                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = {"application/problem+json"}
                        };
                    };
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            AddAuthentication(services);
            AddAuthorization(services);
            AddEventBus(services);
            AddSwagger(services);

            services.AddMediatR(typeof(Startup));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

            var connectionString = _config.GetValue<string>("ConnectionString");

            static void ConfigureNpgsql(NpgsqlDbContextOptionsBuilder npgsqlOptions) =>
                npgsqlOptions.EnableRetryOnFailure();

            services.AddDbContext<OrdersContext>(options =>
            {
                options.UseNpgsql(connectionString, ConfigureNpgsql);
            });

            services.AddDbContext<IntegrationEventLogContext>((sp, options) =>
            {
                var context = sp.GetRequiredService<OrdersContext>();
                var dbConnection = context.Database.GetDbConnection();

                options.UseNpgsql(dbConnection, ConfigureNpgsql);
            });

            services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            services.AddScoped<IOrderQueries, OrderQueries>(_ => new OrderQueries(connectionString));

            services.AddTransient<OrderStatusChangedIntegrationEventHandler>();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = _config["identityUrlInternal"];
                    options.Audience = "orders";
                    options.TokenValidationParameters.ValidIssuers = _config.GetSection("validIssuers")
                        .GetChildren().Select(s => s.Value).ToArray();

                    options.RequireHttpsMetadata = false;
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

        private static void AddEventBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var subManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

                return new RabbitMQEventBus(connection, logger, serviceScopeFactory, subManager, "Orders", 3);
            });

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
                var factory = new ConnectionFactory();
                return new RabbitMQConnection(factory, logger, 3);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "Orders API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new[] { "orders" }, new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flow = OpenApiOAuth2Flow.Implicit,
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
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "ordersswaggerui"
                };
            });

            app.UseEndpoints(builder =>
            {
                builder
                    .MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }
}
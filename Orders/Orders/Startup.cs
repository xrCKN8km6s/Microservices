using System;
using System.Collections.Generic;
using System.Data.Common;
using Common;
using EventBus;
using EventBus.Abstractions;
using EventBus.RabbitMQ;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using IntegrationEventLog.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using Orders.Application.Behaviors;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.EventHandlers;
using Orders.Application.Queries;
using Orders.Domain.Aggregates.Order;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;
using RabbitMQ.Client;
using Users.Client;
using Users.Client.Contracts;

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
            services.AddMvc(options =>
                {
                    var policy = ScopePolicy.Create("orders");
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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

            services.AddTransient<OrderStatusChangedIntegrationEventHandler>();

            services.AddScoped<IOrderQueries, OrderQueries>(_ => new OrderQueries(connectionString));

            services.AddCors();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:3000";
                    options.ApiName = "orders";
                    options.ApiSecret = "orders.secret";
                    options.RequireHttpsMetadata = false; //dev
                });

            services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost"; });

            services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "Orders API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.OAuth2,
                        Flow = SwaggerOAuth2Flow.Implicit,
                        AuthorizationUrl = "http://localhost:3000/connect/authorize",
                        TokenUrl = "http://localhost:3000/connect/token",
                        Scopes = new Dictionary<string, string>
                        {
                            {"orders", "Orders"}
                        }
                    })
                );

                document.OperationProcessors.Add(
                    new OperationSecurityScopeProcessor("oauth2"));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewOrders",
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.ViewOrders.Name));
            });

            services.AddHttpContextAccessor();

            services.AddHttpClient<ITokenAccessor, TokenAccessor>(c => c.BaseAddress = new Uri("http://localhost:3000/connect/token"));

            services.AddClient<IUsersClient, UsersClient>("http://localhost:5100", new ClientConfiguration
            {
                ClientId = "orders",
                ClientSecret = "orders.secret",
                Scope = "users"
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .WithOrigins(_config.GetValue<string>("WebUrl"))
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
                builder => { builder.UseMiddleware<UserProfileMiddleware>(); });

            app.UseSwagger();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "ordersswaggerui"
                };
            });

            app.UseMvc();
        }
    }

    public static class ClientExtensions
    {
        public static void AddClient<TI, TC>(this IServiceCollection services, string baseAddress, ClientConfiguration config)
            where TI : class where TC : class, TI
        {
            services.AddHttpClient<TI, TC>(c => { c.BaseAddress = new Uri(baseAddress); })
                .AddHttpMessageHandler(sp =>
                {
                    var tokenAccessor = sp.GetRequiredService<ITokenAccessor>();
                    return new BearerTokenDelegatingHandler(tokenAccessor, config);
                });
        }
    }
}
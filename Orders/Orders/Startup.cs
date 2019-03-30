using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using Common;
using EventBus;
using EventBus.Abstractions;
using EventBus.RabbitMQ;
using IdentityModel;
using IdentityModel.Client;
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
using Orders.Application.Behaviors;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.EventHandlers;
using Orders.Application.Queries;
using Orders.Domain.Aggregates.Order;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;

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

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:3000";
                    options.ApiName = "orders";
                    options.ApiSecret = "orders.secret";
                    options.RequireHttpsMetadata = false; //dev

                    //options.EnableCaching = true;
                    //options.CacheDuration = TimeSpan.FromSeconds(20);
                });

            services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost"; });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info {Title = "Orders API", Version = "v1"});

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = "http://localhost:3000/connect/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                        {"orders", "Orders API"}
                    }
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"oauth2", new[] {"orders"}}
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewOrders",
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.ViewOrders.Name));
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

            //TODO: this will go to BFF and to Users.Client
            app.Use(async (context, next) =>
            {
                //request user profile from users
                var sub = context.User.FindFirst(JwtClaimTypes.Subject).Value;

                var tokenEndpoint = "http://localhost:3000/connect/token";

                var httpClient = new HttpClient();

                var token = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = "orders",
                    ClientSecret = "orders.secret",
                    Scope = "users"
                });

                httpClient.SetBearerToken(token.AccessToken);

                var resp = await httpClient.GetAsync($"http://localhost:5100/api/users/profile/{sub}");
                var r = await resp.Content.ReadAsAsync<UserProfileDto>();

                var claims = new List<Claim> {new Claim("UserId", r.Id.ToString())};

                claims.AddRange(r.Permissions.Select(s => new Claim(JwtClaimTypes.Role, s.Name)));

                var customIdentity = new ClaimsIdentity(claims);

                context.User.AddIdentity(customIdentity);

                await next.Invoke();
            });

            app.UseMvc();

            app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API");
                    options.OAuthClientId("ordersswaggerui");
                    options.OAuthAppName("Orders Swagger UI");
                });
        }
    }

    //TODO: this will go to Users.Client
    public class UserProfileDto
    {
        public long Id { get; set; }

        public string Sub { get; set; }

        public bool HasGlobalRole { get; set; }

        public PermissionDto[] Permissions { get; set; }
    }

    //TODO: this will go to Users.Client
    public class PermissionDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Orders.Client;
using Orders.Client.Contracts;
using Users.Client;
using Users.Client.Contracts;

namespace BFF
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                    options.Filters.Add(
                        new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError))
                )
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });

            var origins = Configuration.GetSection("origins")
                .GetChildren().Select(s => s.Value).ToArray();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services
                .AddHttpContextAccessor()
                .AddStackExchangeRedisCache(options => { options.Configuration = Configuration["redisConfig"]; });

            AddAuthentication(services);
            AddAuthorization(services);

            AddClients(services);

            AddSwagger(services);

            services
                .AddGrpcClient<Microservices.Users.UsersClient>(c =>
                {
                    c.Address = new Uri(Configuration["clients:users:baseUrl"]);
                })
                .AddHttpMessageHandler<BearerTokenDelegatingHandler>();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["identityUrlInternal"];

                    options.ApiName = "bff";
                    options.ApiSecret = "bff.introspection.secret";

                    options.EnableCaching = true;
                    options.CacheKeyPrefix = "introspection:";
                });
        }

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope("bff")
                    .Build();

                static Action<AuthorizationPolicyBuilder> ConfigurePolicy(string role)
                {
                    return policy => policy
                        .RequireAuthenticatedUser()
                        .RequireScope("bff")
                        .RequireRole(role);
                }

                options.AddPolicy(AuthorizePolicies.OrdersView, ConfigurePolicy(AuthorizePolicies.OrdersView));
                options.AddPolicy(AuthorizePolicies.OrdersEdit, ConfigurePolicy(Permission.OrdersEdit));

                options.AddPolicy(AuthorizePolicies.AdminView, ConfigurePolicy(Permission.AdminView));
                options.AddPolicy(AuthorizePolicies.AdminRolesView, ConfigurePolicy(Permission.AdminRolesView));
                options.AddPolicy(AuthorizePolicies.AdminRolesEdit, ConfigurePolicy(Permission.AdminRolesEdit));
                options.AddPolicy(AuthorizePolicies.AdminRolesDelete, ConfigurePolicy(Permission.AdminRolesDelete));
                options.AddPolicy(AuthorizePolicies.AdminUsersView, ConfigurePolicy(Permission.AdminUsersView));
                options.AddPolicy(AuthorizePolicies.AdminUsersEdit, ConfigurePolicy(Permission.AdminUsersEdit));
            });
        }

        private void AddClients(IServiceCollection services)
        {
            //Move token accessor to separate method.
            services.Configure<TokenAccessorOptions>(Configuration.GetSection("tokenAccessor"));

            services.AddHttpClient("tokenClient", c =>
                    c.BaseAddress = new Uri($"{Configuration["identityUrlInternal"]}/connect/token"))
                .AddTypedClient<ITokenAccessor, TokenAccessor>();

            services.AddTransient<BearerTokenDelegatingHandler>();

            services.AddClient<IUsersClient, UsersClient>(Configuration["clients:users:baseUrl"]);
            services.AddClient<IOrdersClient, OrdersClient>(Configuration["clients:orders:baseUrl"]);
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddOpenApiDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "BFF API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new[] { "bff" }, new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flow = OpenApiOAuth2Flow.AccessCode,
                        AuthorizationUrl = $"{Configuration["identityUrl"]}/connect/authorize",
                        TokenUrl = $"{Configuration["identityUrl"]}/connect/token",
                        Scopes = new Dictionary<string, string>
                        {
                            {"bff", "BFF"}
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            });

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseCors();

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "bffswaggerui",
                    AppName = "BFF",
                    UsePkceWithAuthorizationCodeGrant = true
                };
            });

            app.UseAuthentication();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
                builder => { builder.UseMiddleware<UserProfileMiddleware>(); });

            app.UseAuthorization();

            app.UseEndpoints(builder =>
            {
                builder
                    .MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }
}

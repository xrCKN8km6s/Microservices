using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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
        }

        private void AddAuthentication(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = Configuration["identityUrlInternal"];
                        options.Audience = "bff";
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters.ValidIssuers = Configuration.GetSection("validIssuers")
                            .GetChildren().Select(s => s.Value).ToArray();

                    }, introspectionOptions =>
                    {
                        introspectionOptions.ClientId = "spa";
                        introspectionOptions.ClientSecret = "bff.api.secret";
                        introspectionOptions.EnableCaching = true;
                        introspectionOptions.CacheKeyPrefix = "introspection_";
                        introspectionOptions.IntrospectionEndpoint =
                            $"{Configuration["identityUrlInternal"]}/connect/introspect";
                    }
                );
        }

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope("bff")
                    .Build();

                options.AddPolicy(AuthorizePolicies.OrdersView, policy => policy.RequireRole(Permission.OrdersView));
                options.AddPolicy(AuthorizePolicies.OrdersEdit, policy => policy.RequireRole(Permission.OrdersEdit));

                options.AddPolicy(AuthorizePolicies.AdminView, policy => policy.RequireRole(Permission.AdminView));
                options.AddPolicy(AuthorizePolicies.AdminRolesView, policy => policy.RequireRole(Permission.AdminRolesView));
                options.AddPolicy(AuthorizePolicies.AdminRolesEdit, policy => policy.RequireRole(Permission.AdminRolesEdit));
                options.AddPolicy(AuthorizePolicies.AdminRolesDelete, policy => policy.RequireRole(Permission.AdminRolesDelete));
                options.AddPolicy(AuthorizePolicies.AdminUsersView, policy => policy.RequireRole(Permission.AdminUsersView));
                options.AddPolicy(AuthorizePolicies.AdminUsersEdit, policy => policy.RequireRole(Permission.AdminUsersEdit));
            });
        }

        private void AddClients(IServiceCollection services)
        {
            var tokenConfig = new TokenAccessorConfiguration
            {
                ClientId = "bff",
                ClientSecret = "bff.client.secret",
                Scopes = Configuration["clients:scopes"]
            };

            services.AddHttpClient("tokenClient", c =>
                    c.BaseAddress = new Uri($"{Configuration["identityUrlInternal"]}/connect/token"))
                .AddTypedClient<ITokenAccessor>((httpClient, sp) =>
                {
                    var cache = sp.GetRequiredService<IDistributedCache>();
                    var logger = sp.GetRequiredService<ILogger<TokenAccessor>>();

                    return new TokenAccessor(httpClient, tokenConfig, cache, logger);
                });

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
                        Flow = OpenApiOAuth2Flow.Implicit,
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
                    ClientSecret = "bff.api.secret"
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

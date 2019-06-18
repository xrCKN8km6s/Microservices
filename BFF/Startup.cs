using System;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
                .AddMvcCore(options => { options.Filters.Add(new AuthorizeFilter(ScopePolicy.Create("bff"))); })
                .AddJsonFormatters()
                .AddJsonOptions(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
                .AddApiExplorer()
                .AddFormatterMappings()
                .AddDataAnnotations()
                .AddAuthorization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            AddAuthentication(services);
            AddAuthorization(services);
            AddClients(services);
            AddSwagger(services);

            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration["redisConfig"]; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(builder => builder
                .WithOrigins(Configuration.GetValue<string>("WebUrl"))
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "bffswaggerui"
                };
            });

            app.UseAuthentication();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"),
                builder => { builder.UseMiddleware<UserProfileMiddleware>(); });

            app.UseMvc();
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "BFF API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new[] {"bff"}, new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flow = OpenApiOAuth2Flow.Implicit,
                        AuthorizationUrl = $"{Configuration["identityUrl"]}/connect/authorize",
                        TokenUrl = $"h{Configuration["identityUrl"]}/connect/token",
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

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizePolicies.OrdersView,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.OrdersView));
                options.AddPolicy(AuthorizePolicies.OrdersEdit,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.OrdersEdit));

                options.AddPolicy(AuthorizePolicies.AdminView,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.AdminView));
                options.AddPolicy(AuthorizePolicies.AdminRolesView,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.AdminRolesView));
                options.AddPolicy(AuthorizePolicies.AdminRolesEdit,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.AdminRolesEdit));
                options.AddPolicy(AuthorizePolicies.AdminRolesDelete,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.AdminRolesDelete));
                options.AddPolicy(AuthorizePolicies.AdminUsersView,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.AdminUsersView));
                options.AddPolicy(AuthorizePolicies.AdminUsersEdit,
                    policy => policy.RequireClaim(JwtClaimTypes.Role, Permission.AdminUsersEdit));
            });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = Configuration["identityUrlInternal"];

                        options.ApiName = "bff";
                        options.ApiSecret = "bff.api.secret";

                        options.EnableCaching = true;

                        options.RequireHttpsMetadata = false;
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

            services.AddHttpClient<ITokenAccessor>(c =>
                    c.BaseAddress = new Uri($"{Configuration["identityUrlInternal"]}/connect/token"))
                .AddTypedClient<ITokenAccessor>((httpClient, sp) =>
                {
                    var cache = sp.GetRequiredService<IDistributedCache>();
                    var logger = sp.GetRequiredService<ILogger<TokenAccessor>>();

                    return new TokenAccessor(httpClient, tokenConfig, cache, logger);
                });

            services.AddClient<IUsersClient, UsersClient>(Configuration["clients:users:baseUrl"]);
            services.AddClient<IOrdersClient, OrdersClient>(Configuration["clients:orders:baseUrl"]);
        }
    }
}

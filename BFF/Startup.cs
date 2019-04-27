using System;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
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
            services.AddMvc(options => { options.Filters.Add(new AuthorizeFilter(ScopePolicy.Create("bff"))); })
                .AddJsonOptions(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
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

            app.UseSwagger();
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
                    new SecurityDefinitionAppender("oauth2", new[] {"bff"}, new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.OAuth2,
                        Flow = SwaggerOAuth2Flow.Implicit,
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
                .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = Configuration["identityUrlInternal"];
                        options.RequireHttpsMetadata = false; //dev

                        options.ApiName = "bff";
                        options.ApiSecret = "bff.api.secret";
                    });

            services.PostConfigure<JwtBearerOptions>(options =>
            {
                options.TokenValidationParameters.ValidIssuers = Configuration.GetValue<string[]>("validIssuers");
            });
        }

        private void AddClients(IServiceCollection services)
        {
            services.AddHttpClient<ITokenAccessor, TokenAccessor>(c =>
                c.BaseAddress = new Uri($"{Configuration["identityUrlInternal"]}/connect/token"));

            services.AddClient<IUsersClient, UsersClient>(Configuration["usersUrl"], new ClientConfiguration
            {
                ClientId = "bff",
                ClientSecret = "bff.client.secret",
                Scope = "users"
            });

            services.AddClient<IOrdersClient, OrdersClient>(Configuration["ordersUrl"], new ClientConfiguration
            {
                ClientId = "bff",
                ClientSecret = "bff.client.secret",
                Scope = "orders"
            });
        }
    }
}

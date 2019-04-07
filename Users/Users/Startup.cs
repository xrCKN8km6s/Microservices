using System;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using Users.Infrastructure;
using Users.Queries;

namespace Users
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var policy = ScopePolicy.Create("users");
                options.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:3000";
                    options.ApiName = "users";
                    options.ApiSecret = "users.secret";
                    options.RequireHttpsMetadata = false; //dev
                });

            services.AddScoped<IUsersQueries, UsersQueries>();

            var connectionString = Configuration.GetValue<string>("ConnectionString");

            services.AddDbContext<UsersContext>(options =>
                options
                    .UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "Users API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.OAuth2,
                        Flow = SwaggerOAuth2Flow.Implicit,
                        AuthorizationUrl = "http://localhost:3000/connect/authorize",
                        TokenUrl = "http://localhost:3000/connect/token",
                        Scopes = new Dictionary<string, string>
                        {
                            {"users", "Users"}
                        }
                    })
                );

                document.OperationProcessors.Add(
                    new OperationSecurityScopeProcessor("oauth2"));
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
                .WithOrigins(Configuration.GetValue<string>("WebUrl"))
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "usersswaggerui"
                };
            });

            app.UseMvc();
        }
    }
}
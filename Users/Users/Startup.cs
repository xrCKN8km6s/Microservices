using System.Collections.Generic;
using System.Linq;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(new AuthorizeFilter(ScopePolicy.Create("users")));
                })
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            AddAuthentication(services);
            AddSwagger(services);

            var connectionString = Configuration.GetValue<string>("ConnectionString");

            services.AddDbContext<UsersContext>(options =>
                options
                    .UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            services.AddScoped<IUsersQueries, UsersQueries>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
        private static void AddAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:3000";
                    options.ApiName = "users";
                    options.ApiSecret = "users.secret";
                    options.RequireHttpsMetadata = false; //dev
                });
        }

        private static void AddSwagger(IServiceCollection services)
        {
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
    }
}
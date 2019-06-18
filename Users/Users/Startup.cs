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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Users.Infrastructure;
using Users.Queries;

namespace Users
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore(options => { options.Filters.Add(new AuthorizeFilter(ScopePolicy.Create("users"))); })
                .AddJsonFormatters()
                .AddJsonOptions(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
                .AddApiExplorer()
                .AddFormatterMappings()
                .AddDataAnnotations()
                .AddAuthorization()
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

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "usersswaggerui"
                };
            });

            app.UseMvc();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["identityUrlInternal"];
                    options.Audience = "users";
                    options.TokenValidationParameters.ValidIssuers = Configuration.GetSection("validIssuers")
                        .GetChildren().Select(s => s.Value).ToArray();

                    options.RequireHttpsMetadata = false;
                });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument(document =>
            {
                document.PostProcess = d => d.Info.Title = "Users API";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("oauth2", new[] {"users"}, new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Flow = OpenApiOAuth2Flow.Implicit,
                        AuthorizationUrl = $"{Configuration["identityUrl"]}/connect/authorize",
                        TokenUrl = $"{Configuration["identityUrl"]}/connect/token",
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
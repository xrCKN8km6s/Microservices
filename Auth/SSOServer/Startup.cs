using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SSOServer
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddIdentityServer()
                .AddTestUsers(new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "123456", Username = "john", Password = "smith", Claims = new List<Claim>
                        {
                            new Claim(JwtClaimTypes.Name, "John Smith"),
                            new Claim(JwtClaimTypes.Email, "John.Smith@example.com")
                        }
                    }
                })
                .AddInMemoryClients(new[]
                {
                    new Client
                    {
                        ClientId = "EntryPoint",
                        ClientSecrets = {new Secret("secret".Sha256())},
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowedScopes = {"openid", "profile", "email", "EntryPoint.ro", "EntryPoint.rw"},
                        RedirectUris =
                            {"http://localhost:4200/signin-callback", "http://localhost:4200/silent-callback"},
                        PostLogoutRedirectUris = {"http://localhost:4200"},
                        AllowAccessTokensViaBrowser = true,
                        AccessTokenType = AccessTokenType.Reference,
                        RequireConsent = false,
                        AllowedCorsOrigins = {"http://localhost:4200"},
                        AccessTokenLifetime = 150
                    }
                })
                .AddInMemoryIdentityResources(new[]
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Email(),
                    new IdentityResources.Profile(),
                    new IdentityResource
                    {
                        Name = "api"
                    },
                })
                .AddInMemoryApiResources(new[]
                {
                    new ApiResource
                    {
                        Name = "api",
                        ApiSecrets = {new Secret("api.secret".Sha256())},
                        UserClaims = {JwtClaimTypes.Name, JwtClaimTypes.Email},
                        Scopes =
                        {
                            new Scope("EntryPoint.ro", "Read-only access"),
                            new Scope("EntryPoint.rw", "Full access")
                        }
                    }
                })
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}
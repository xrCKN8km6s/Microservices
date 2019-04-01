// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                new ApiResource("orders", "Orders API")
                {
                    ApiSecrets = {new Secret("orders.secret".Sha256())},
                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email
                    }
                },
                new ApiResource("users", "Users API")
                {
                    ApiSecrets = {new Secret("users.secret".Sha256())}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Reference,
                    RequireClientSecret = false, //using secret in SPA is not secure anyway

                    RedirectUris =
                    {
                        "http://localhost:4200/signin-callback",
                        "http://localhost:4200/assets/silent-callback.html"
                    },

                    PostLogoutRedirectUris = {"http://localhost:4200"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    RequireConsent = false,

                    AllowedScopes = {"openid", "profile", "email", "orders", "users"},
                    AccessTokenLifetime = 100
                },

                new Client
                {
                    ClientId = "ordersswaggerui",
                    ClientName = "Orders Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris = {"http://localhost:5000/swagger/oauth2-redirect.html"},

                    AllowedScopes = {"orders"}
                },

                new Client
                {
                    ClientId = "usersswaggerui",
                    ClientName = "Users Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris = {"http://localhost:5100/swagger/oauth2-redirect.html"},

                    AllowedScopes = {"users"}
                },

                new Client
                {
                    ClientId = "orders",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("orders.secret".Sha256())
                    },

                    AllowedScopes = {"users"}
                }
            };
        }
    }
}
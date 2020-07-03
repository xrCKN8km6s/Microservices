// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> ApiScopes =>

            new List<ApiScope>
            {
                new ApiScope("orders", "Orders API Access Scope"),
                new ApiScope("users", "Users API Access Scope"),
                new ApiScope("bff", "BFF API Access Scope")
                {
                    UserClaims =
                    {
                        JwtClaimTypes.Subject,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email
                    }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("bff", "BFF API Resource")
                {
                    Scopes = {"bff"}
                },
                new ApiResource("users", "Users API Resource")
                {
                    Scopes = {"users"}
                },
                new ApiResource("orders", "Orders API Resource")
                {
                    Scopes = {"orders"}
                }
            };


        public static IEnumerable<Client> Clients =>

          new List<Client>
            {
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    //AccessTokenType = AccessTokenType.Reference,
                    RequireClientSecret = false, //using secret in SPA is not secure anyway

                    RedirectUris =
                    {
                        "http://localhost:4200/signin-callback",
                        "http://localhost:4200/assets/silent-callback.html"
                    },

                    PostLogoutRedirectUris = {"http://localhost:4200"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    RequireConsent = false,

                    AllowedScopes = {"openid", "profile", "email", "bff"}
                },

                new Client
                {
                    ClientId = "bffswaggerui",
                    ClientSecrets = {new Secret("bff.api.secret".Sha256())},
                    ClientName = "BFF Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        "http://localhost:5000/swagger/oauth2-redirect.html",
                        "https://localhost:5001/swagger/oauth2-redirect.html",
                        "https://localhost:1443/swagger/oauth2-redirect.html"
                    },

                    AllowedScopes = {"bff"}
                },

                new Client
                {
                    ClientId = "ordersswaggerui",
                    ClientSecrets = {new Secret("orders.secret".Sha256())},
                    ClientName = "Orders Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        "http://localhost:5200/swagger/oauth2-redirect.html",
                        "https://localhost:5201/swagger/oauth2-redirect.html"
                    },

                    AllowedScopes = {"orders"}
                },

                new Client
                {
                    ClientId = "usersswaggerui",
                    ClientSecrets = {new Secret("users.secret".Sha256())},
                    ClientName = "Users Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        "http://localhost:5100/swagger/oauth2-redirect.html",
                        "https://localhost:5101/swagger/oauth2-redirect.html"
                    },

                    AllowedScopes = {"users"}
                },

                new Client
                {
                    ClientId = "bff",
                    ClientSecrets = {new Secret("bff.client.secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = {"users", "orders"}
                }
            };
    };
}
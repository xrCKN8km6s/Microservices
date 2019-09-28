using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Users.Client.Contracts;

namespace BFF
{
    //TODO: logging/error handling
    public class UserProfileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUsersClient _usersClient;
        private readonly IDistributedCache _cache;

        public UserProfileMiddleware(RequestDelegate next, IUsersClient usersClient, IDistributedCache cache)
        {
            _next = next;
            _usersClient = usersClient;
            _cache = cache;
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext context)
        {
            var subClaim = context.User.FindFirst(JwtClaimTypes.Subject);

            if (subClaim == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var profileCacheKey = $"profile_{subClaim.Value}";

            var cached = await _cache.GetStringAsync(profileCacheKey);

            UserProfileDto profile;

            if (string.IsNullOrWhiteSpace(cached))
            {
                profile = await _usersClient.Profile_GetUserProfileAsync(subClaim.Value);
                var content = JsonConvert.SerializeObject(profile);
                await _cache.SetStringAsync(profileCacheKey, content,
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)});
            }
            else
            {
                profile = JsonConvert.DeserializeObject<UserProfileDto>(cached);
            }

            PrepareIdentity(context, profile);

            await _next(context);
        }

        private static void PrepareIdentity(HttpContext context, UserProfileDto profile)
        {
            var claims = new List<Claim> { new Claim("UserId", profile.Id.ToString()) };

            claims.AddRange(profile.Permissions.Select(s => new Claim(JwtClaimTypes.Role, s.Name)));

            var customIdentity = new ClaimsIdentity(claims, "CustomAuthentication", JwtClaimTypes.Name, JwtClaimTypes.Role);

            context.User.AddIdentity(customIdentity);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Users.Client.Contracts;

namespace BFF
{
    public class UserProfileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUsersClient _usersClient;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UserProfileMiddleware> _logger;

        public UserProfileMiddleware(RequestDelegate next, IUsersClient usersClient,
            IDistributedCache cache, ILogger<UserProfileMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _usersClient = usersClient ?? throw new ArgumentNullException(nameof(usersClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var sub = context.User.FindFirstValue(JwtClaimTypes.Subject);

            if (sub == null)
            {
                _logger.LogError("sub claim is missing.");
                context.Response.StatusCode = 401;
                return;
            }

            var profileCacheKey = $"profile_{sub}";

            var cached = await _cache.GetAsync(profileCacheKey);

            UserProfileDto profile;

            if (cached == null)
            {
                profile = await _usersClient.Profile_GetUserProfileAsync(sub);

                var exp = context.User.FindFirstValue(JwtClaimTypes.Expiration);

                if (long.TryParse(exp, out var unixEpochSecs))
                {
                    var identityExp = DateTimeOffset.FromUnixTimeSeconds(unixEpochSecs);

                    var offset = DateTimeOffset.UtcNow.AddMinutes(10);

                    var absExp = offset > identityExp ? identityExp : offset;

                    var content = JsonSerializer.SerializeToUtf8Bytes(profile);

                    await _cache.SetAsync(profileCacheKey, content,
                        new DistributedCacheEntryOptions { AbsoluteExpiration = absExp });
                }
            }
            else
            {
                profile = JsonSerializer.Deserialize<UserProfileDto>(cached);
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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Users.Client.Contracts;

namespace Orders
{
    //TODO: logging/error handling
    public class UserProfileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUsersClient _usersClient;

        public UserProfileMiddleware(RequestDelegate next, IUsersClient usersClient)
        {
            _next = next;
            _usersClient = usersClient;
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext context)
        {
            var sub = context.User.FindFirst(JwtClaimTypes.Subject).Value;
            var profile = await _usersClient.GetUserAsync(sub);

            var claims = new List<Claim> { new Claim("UserId", profile.Id.ToString()) };

            claims.AddRange(profile.Permissions.Select(s => new Claim(JwtClaimTypes.Role, s.Name)));

            var customIdentity = new ClaimsIdentity(claims);

            context.User.AddIdentity(customIdentity);

            await _next(context);
        }
    }
}
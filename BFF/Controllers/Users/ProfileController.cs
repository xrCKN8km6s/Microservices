using System.Threading.Tasks;
using Clients.Common;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Client.Contracts;

namespace BFF.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUsersClient _client;

        public ProfileController(IUsersClient client)
        {
            _client = client;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<ActionResult<UserProfileDto>> Get()
        {
            var sub = User.FindFirst(JwtClaimTypes.Subject).Value;
            return await _client.Profile_GetUserProfileAsync(sub, HttpContext.RequestAborted);
        }
    }
}

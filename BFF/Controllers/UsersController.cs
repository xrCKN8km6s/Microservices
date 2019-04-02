using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Users.Client.Contracts;

namespace BFF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersClient _client;

        public UsersController(IUsersClient client)
        {
            _client = client;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> Get()
        {
            var sub = User.FindFirst(JwtClaimTypes.Subject).Value;
            return await _client.GetUserAsync(sub, HttpContext.RequestAborted);
        }
    }
}

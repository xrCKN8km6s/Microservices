using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Users.Client.Contracts;

namespace BFF.Controllers.Users
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
            return await _client.Users_GetUserAsync(sub, HttpContext.RequestAborted);
        }

        [HttpGet("viewmodel")]
        public async Task<ActionResult<UsersViewModel>> GetViewModel()
        {
            return await _client.Users_GetViewModelAsync(HttpContext.RequestAborted);
        }

        [HttpPut("{id}/roles")]
        public async Task<ActionResult> UpdateUserRoles([FromRoute] long id, [FromBody] UpdateUserRolesDto roles)
        {
            await _client.Users_UpdateUserRolesAsync(id, roles, HttpContext.RequestAborted);
            return NoContent();
        }
    }
}

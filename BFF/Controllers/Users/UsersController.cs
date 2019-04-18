using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Users.Client.Contracts;

namespace BFF.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersClient _client;
        private readonly IDistributedCache _cache;

        public UsersController(IUsersClient client, IDistributedCache cache)
        {
            _client = client;
            _cache = cache;
        }

        [HttpGet("viewmodel")]
        [Authorize(Policy = AuthorizePolicies.AdminUsersView)]
        public async Task<ActionResult<UsersViewModel>> GetViewModel()
        {
            return await _client.Users_GetViewModelAsync(HttpContext.RequestAborted);
        }

        [HttpPut("{id}/roles")]
        [Authorize(Policy = AuthorizePolicies.AdminUsersEdit)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> UpdateUserRoles([FromRoute] long id, [FromBody] UpdateUserRolesDto roles)
        {
            await _client.Users_UpdateUserRolesAsync(id, roles, HttpContext.RequestAborted);
            //TODO: refactor
            await _cache.RemoveAsync($"profile_{User.FindFirst(JwtClaimTypes.Subject).Value}");
            return NoContent();
        }
    }
}

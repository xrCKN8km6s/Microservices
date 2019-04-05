using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Client.Contracts;

namespace BFF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUsersClient _client;

        public RolesController(IUsersClient client)
        {
            _client = client;
        }

        [HttpGet("viewmodel")]
        public async Task<ActionResult<RolesViewModel>> Get()
        {
            var res = await _client.Roles_GetRolesViewModelAsync(HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var res = await _client.Roles_GetRolesAsync(HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(long id)
        {
            var res = await _client.Roles_GetRoleByIdAsync(id, HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(long id)
        {
            await _client.Roles_DeleteRoleAsync(id, HttpContext.RequestAborted);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleDto role)
        {
            await _client.Roles_CreateRoleAsync(role, HttpContext.RequestAborted);
            return NoContent();
        }
    }
}

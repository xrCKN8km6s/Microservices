using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Client.Contracts;

namespace BFF.Controllers.Users
{
    [Route("api/grpc/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthorizePolicies.AdminRolesView)]
    public class GrpcRolesController : ControllerBase
    {
        private readonly Microservices.Users.UsersClient _users;

        public GrpcRolesController(Microservices.Users.UsersClient users)
        {
            _users = users;
        }

        //[HttpPost]
        //[Authorize(Policy = AuthorizePolicies.AdminRolesEdit)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Clients.Common.ValidationProblemDetails))]
        //public async Task<ActionResult> CreateRole([FromBody] CreateEditRoleDto role)
        //{
        //    await _client.Roles_CreateRoleAsync(role, HttpContext.RequestAborted);
        //    return NoContent();
        //}

        //[HttpPut("{id}")]
        //[Authorize(Policy = AuthorizePolicies.AdminRolesEdit)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Clients.Common.ValidationProblemDetails))]
        //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Clients.Common.ProblemDetails))]
        //public async Task<ActionResult> UpdateRole([FromRoute] long id, [FromBody] CreateEditRoleDto role)
        //{
        //    await _client.Roles_UpdateRoleAsync(id, role);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //[Authorize(Policy = AuthorizePolicies.AdminRolesDelete)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Clients.Common.ProblemDetails))]
        //public async Task<ActionResult> DeleteRole(long id)
        //{
        //    await _client.Roles_DeleteRoleAsync(id, HttpContext.RequestAborted);
        //    return NoContent();
        //}

        [HttpGet("")]
        [Authorize(Policy = AuthorizePolicies.AdminRolesView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleReply>>> GetRoles()
        {
            var res = await _users.GetRolesAsync(new Empty());
            return Ok(res.Roles);
        }

        //[HttpGet("viewmodel")]
        //[Authorize(Policy = AuthorizePolicies.AdminRolesView)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<RolesViewModel>> GetRolesViewModel()
        //{
        //    var res = await _client.Roles_GetRolesViewModelAsync(HttpContext.RequestAborted);
        //    return Ok(res);
        //}

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Clients.Common.ProblemDetails))]
        public async Task<ActionResult<RoleReply>> GetRoleById(long id)
        {
            var res = await _users.GetRoleByIdAsync(new Int64Value {Value = id});
            return Ok(res);
        }
    }
}

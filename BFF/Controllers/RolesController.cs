﻿using System.Collections.Generic;
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

        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateEditRoleDto role)
        {
            await _client.Roles_CreateRoleAsync(role, HttpContext.RequestAborted);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole([FromRoute] long id, [FromBody] CreateEditRoleDto role)
        {
            await _client.Roles_UpdateRoleAsync(id, role);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(long id)
        {
            await _client.Roles_DeleteRoleAsync(id, HttpContext.RequestAborted);
            return NoContent();
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var res = await _client.Roles_GetRolesAsync(HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpGet("viewmodel")]
        public async Task<ActionResult<RolesViewModel>> GetRolesViewModel()
        {
            var res = await _client.Roles_GetRolesViewModelAsync(HttpContext.RequestAborted);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(long id)
        {
            var res = await _client.Roles_GetRoleByIdAsync(id, HttpContext.RequestAborted);
            return Ok(res);
        }
    }
}

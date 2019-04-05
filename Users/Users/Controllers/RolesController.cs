using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Users.DTO;
using Users.Infrastructure;
using Users.Queries;

namespace Users.Controllers
{
    //TODO: refactor context
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUsersQueries _queries;
        private readonly UsersContext _context;

        public RolesController(IUsersQueries queries, UsersContext context)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _context = context;
        }

        [HttpGet("viewmodel")]
        public async Task<ActionResult<RolesViewModel>> GetRolesViewModel()
        {
            var res = await _queries.GetRolesViewModelAsync();

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.ToArrayAsync();

            var dto = roles.Select(MapRoleToDto);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(long id)
        {
            var role = await _context.Roles.Include(i => i.PermissionRoles).FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(MapRoleToDto(role));
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(typeof(void))]
        public async Task<ActionResult> DeleteRole(long id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            role.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [SwaggerResponse(typeof(void))]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleDto role)
        {
            var newRole = MapCreateDtoToRole(role);
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        [SwaggerResponse(typeof(void))]
        public async Task<ActionResult> UpdateRole([FromBody] RoleDto role)
        {
            var dbRole = await _context.Roles.Include(i => i.PermissionRoles).FirstOrDefaultAsync(r => r.Id == role.Id);
            if (dbRole == null)
            {
                return NotFound();
            }

            if (role.IsGlobal && role.Permissions.Length > 0)
            {
                return BadRequest("Global role can't have permissions");
            }

            dbRole.Name = role.Name;

            if (!dbRole.IsGlobal && role.IsGlobal)
            {
                dbRole.PermissionRoles.Clear();
            }

            dbRole.IsGlobal = role.IsGlobal;

            var permissions = role.Permissions
                .Select(s => Permission.TryParse(s, out var permission) ? permission : null)
                .Where(w => w != null);

            dbRole.PermissionRoles.Clear();
            dbRole.PermissionRoles.AddRange(permissions.Select(p => new PermissionRole
            {
                Permission = p
            }));

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static Role MapCreateDtoToRole(CreateRoleDto dto)
        {
            return new Role
            {
                Name = dto.Name,
                IsGlobal = dto.IsGlobal,
                IsActive = true,
                PermissionRoles = dto.Permissions.Select(p => new PermissionRole
                {
                    Permission = Permission.Parse(p)
                }).ToList()
            };
        }

        private static RoleDto MapRoleToDto(Role role)
        {
            var dto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                IsGlobal = role.IsGlobal
            };

            if (role.PermissionRoles.Count > 0)
            {
                dto.Permissions = role.PermissionRoles.Select(s => s.Permission.Id).ToArray();
            }

            return dto;
        }

        private static PermissionDto MapPermissionToDto(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            return Ok(res);
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.AsNoTracking().ToArrayAsync();
            var dto = roles.Select(MapRoleToDto);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<RoleDto>> GetRoleById(long id)
        {
            var role = await _context.Roles.AsNoTracking().Include(i => i.PermissionRoles).FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound($"Role {id} was not found.");
            }

            return MapRoleToDto(role);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> DeleteRole(long id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound($"Role {id} was not found.");
            }

            role.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateRole([FromBody] CreateEditRoleDto role)
        {
            var newRole = MapCreateDtoToRole(role);
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> UpdateRole([FromRoute] long id, [FromBody] CreateEditRoleDto role)
        {
            var dbRole = await _context.Roles.Include(i => i.PermissionRoles).FirstOrDefaultAsync(r => r.Id == id);
            if (dbRole == null)
            {
                return NotFound($"Role {id} was not found.");
            }

            var allExistingPermissionIds = Enumeration.GetAll<Permission>().Select(s => s.Id);

            var notFoundPermissions = role.Permissions.Except(allExistingPermissionIds).ToArray();
            if (notFoundPermissions.Length > 0)
            {
                return NotFound($"Permissions {{{string.Join(',', notFoundPermissions)}}} were not found");
            }

            if (!dbRole.IsGlobal && role.IsGlobal)
            {
                dbRole.PermissionRoles.Clear();
            }

            dbRole.Name = role.Name;
            dbRole.IsGlobal = role.IsGlobal;

            var dbRolePermissions = dbRole.PermissionRoles.Select(s => s.Permission).ToArray();

            var permissions = role.Permissions
                .Select(s => Enumeration.TryParse<Permission>(s, out var permission) ? permission : null)
                .Where(w => w != null)
                .ToArray();

            var permissionsToAdd = permissions.Except(dbRolePermissions);
            var permissionsToRemove = dbRolePermissions.Except(permissions).ToHashSet();

            dbRole.PermissionRoles.AddRange(permissionsToAdd.Select(p => new PermissionRole {Permission = p}));
            dbRole.PermissionRoles.RemoveAll(pr => permissionsToRemove.Contains(pr.Permission));

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static Role MapCreateDtoToRole(CreateEditRoleDto dto)
        {
            return new Role
            {
                Name = dto.Name,
                IsGlobal = dto.IsGlobal,
                IsActive = true,
                PermissionRoles = dto.Permissions.Select(p => new PermissionRole
                {
                    Permission = Enumeration.Parse<Permission>(p)
                }).ToList()
            };
        }

        public static RoleDto MapRoleToDto(Role role)
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
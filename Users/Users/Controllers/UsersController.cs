using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Users.DTO;
using Users.Infrastructure;
using Users.Queries;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersQueries _queries;
        private readonly UsersContext _context;

        public UsersController(IUsersQueries queries, [NotNull] UsersContext context)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("profile/{sub}")]
        public async Task<ActionResult<UserProfileDto>> GetUser(string sub)
        {
            var res = await _queries.GetUserProfileAsync(sub);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet("viewmodel")]
        public async Task<ActionResult<UsersViewModel>> GetViewModel()
        {
            var users = await _context.Users.AsNoTracking().Include(i => i.UserRoles).ThenInclude(i => i.Role).ToArrayAsync();
            var roles = await _context.Roles.AsNoTracking().ToArrayAsync();

            return new UsersViewModel
            {
                Users = users.Select(MapUserToDto).ToArray(),
                Roles = roles.Select(RolesController.MapRoleToDto).ToArray()
            };
        }

        [HttpPut("{id}/roles")]
        [SwaggerResponse(typeof(void))]
        public async Task<ActionResult> UpdateUserRoles([FromRoute] long id, [FromBody] UpdateUserRolesDto roles)
        {
            var user = await _context.Users.Include(i => i.UserRoles).FirstOrDefaultAsync(f => f.Id == id);
            if (user == null)
            {
                return NoContent();
            }

            var dbAllRoles = await _context.Roles.Select(r => r.Id).ToArrayAsync();

            var invalidRoles = roles.Roles.Except(dbAllRoles).ToArray();
            if (invalidRoles.Length > 0)
            {
                return BadRequest($"Roles {string.Join(',', invalidRoles)} do not exist.");
            }

            var dbUserRoles = user.UserRoles.Select(s => s.RoleId).ToArray();

            var rolesToAdd = roles.Roles.Except(dbUserRoles).ToArray();
            var rolesToRemove = dbUserRoles.Except(roles.Roles).ToArray();

            user.UserRoles.AddRange(rolesToAdd.Select(s => new UserRole {RoleId = s}));
            user.UserRoles.RemoveAll(ur => rolesToRemove.Contains(ur.RoleId));

            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class UserDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long[] Roles { get; set; }
        }


        public class UpdateUserRolesDto
        {
            public long[] Roles { get; set; }
        }

        public class UsersViewModel
        {
            public UserDto[] Users { get; set; }
            public RoleDto[] Roles { get; set; }
        }

        public static UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Roles = user.UserRoles.Select(s => s.RoleId).ToArray()
            };
        }
    }
}
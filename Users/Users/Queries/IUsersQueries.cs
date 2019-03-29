using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.EntityFrameworkCore;
using Users.Infrastructure;
using Users.Models;

namespace Users.Queries
{
    public interface IUsersQueries
    {
        Task<UserProfileDto> GetUserProfileAsync(string sub);
    }

    public class UsersQueries : IUsersQueries
    {
        private readonly UsersContext _context;

        public UsersQueries(UsersContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string sub)
        {
            var user = await _context.Users.AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(t => t.Role)
                .ThenInclude(t => t.PermissionRoles)
                .FirstOrDefaultAsync(f => f.Sub == sub);

            return MapToDto(user);
        }

        private static UserProfileDto MapToDto(User user)
        {
            var hasGlobalRole = user.UserRoles.Any(a => a.Role.IsGlobal);
            return new UserProfileDto
            {
                Id = user.Id,
                Sub = user.Sub,
                HasGlobalRole = hasGlobalRole,
                Permissions = hasGlobalRole
                    ? Permission.GetAll().Select(MapPermissionToDto).ToArray()
                    : user.UserRoles.Select(s => s.Role)
                        .SelectMany(s => s.PermissionRoles)
                        .Select(s => MapPermissionToDto(s.Permission)).ToArray()
            };
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

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class UserProfileDto
    {
        public long Id { get; set; }

        public string Sub { get; set; }

        public bool HasGlobalRole { get; set; }

        public PermissionDto[] Permissions { get; set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class PermissionDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
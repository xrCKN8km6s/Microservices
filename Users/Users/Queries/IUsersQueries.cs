﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.EntityFrameworkCore;
using Users.DTO;
using Users.Infrastructure;

namespace Users.Queries
{
    //TODO: remove mappings, convert to repository
    public interface IUsersQueries
    {
        Task<UserProfileDto> GetUserProfileAsync(string sub);
        Task<RolesViewModel> GetRolesViewModelAsync();
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

            return user == null ? null : MapUserToDto(user);
        }

        public async Task<RolesViewModel> GetRolesViewModelAsync()
        {
            var roles = await _context.Roles.AsNoTracking()
                .Include(i => i.PermissionRoles)
                .ToArrayAsync();

            return new RolesViewModel
            {
                Roles = roles.Select(MapRoleToDto).ToArray(),
                AllPermissions = Permission.GetAll().Select(MapPermissionToDto).ToArray()
            };
        }

        private static UserProfileDto MapUserToDto(User user)
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
                        .Select(s => MapPermissionToDto(s.Permission))
                        .Distinct()
                        .ToArray()
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

        private static RoleDto MapRoleToDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                IsGlobal = role.IsGlobal,
                Permissions = role.PermissionRoles.Select(s => s.Permission.Id).ToArray()
            };
        }
    }
}
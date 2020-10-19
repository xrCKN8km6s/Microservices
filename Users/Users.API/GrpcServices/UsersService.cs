using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users.API.DTO;
using Users.API.Infrastructure;
using Users.API.Queries;

namespace Users.API.GrpcServices
{
    public class UsersService : Microservices.Users.UsersBase
    {
        private readonly ILogger<UsersService> _logger;
        private readonly IUsersQueries _queries;
        private readonly UsersContext _context;

        public UsersService(ILogger<UsersService> logger, IUsersQueries queries, UsersContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Authorize]
        public override async Task<UserProfileReply> GetProfile(ProfileRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Requesting profile for user {sub}", request?.Sub);
            if (string.IsNullOrWhiteSpace(request?.Sub))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid argument."));
            }

            var res = await _queries.GetUserProfileAsync(request.Sub);
            if (res == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"User profile {request.Sub} was not found."));
            }

            return MapReply(res);
        }

        private static UserProfileReply MapReply(UserProfileDto dto)
        {
            var res = new UserProfileReply
            {
                Id = dto.Id,
                Sub = dto.Sub,
                Name = dto.Name,
                Email = dto.Email,
                HasGlobalRole = dto.HasGlobalRole
            };

            res.Permissions.AddRange(dto.Permissions.Select(s => new UserProfileReply.Types.PermissionReply
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            }));

            return res;
        }

        public override async Task<RoleReply> GetRoleById(IdRequest request, ServerCallContext context)
        {
            if (request?.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"{request?.Id} is invalid."));
            }

            var role = await _context.Roles.AsNoTracking().Include(i => i.PermissionRoles).FirstOrDefaultAsync(r => r.Id == request.Id);
            if (role == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Role {request?.Id} was not found."));
            }

            return MapRoleToDto(role);
        }

        public override async Task<RolesReply> GetRoles(Empty request, ServerCallContext context)
        {
            var roles = await _context.Roles.AsNoTracking().ToArrayAsync();
            var dto = roles.Select(MapRoleToDto);

            return new RolesReply
            {
                Roles = { dto }
            };
        }

        public static RoleReply MapRoleToDto(Role role)
        {
            _ = role ?? throw new ArgumentNullException(nameof(role));

            var dto = new RoleReply
            {
                Id = role.Id,
                Name = role.Name,
                IsGlobal = role.IsGlobal
            };

            if (role.PermissionRoles.Count > 0)
            {
                dto.Permissions.AddRange(role.PermissionRoles.Select(s => s.Permission.Id));
            }

            return dto;
        }
    }
}

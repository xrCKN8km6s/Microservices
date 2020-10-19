using System.Threading.Tasks;
using IdentityModel;
using Microservices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BFF.Controllers.Users
{
    [Route("api/grpc/[controller]")]
    [ApiController]
    public class GrpcProfileController : ControllerBase
    {
        private readonly Microservices.Users.UsersClient _users;

        public GrpcProfileController(Microservices.Users.UsersClient users)
        {
            _users = users;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Clients.Common.ProblemDetails))]
        public async Task<ActionResult<UserProfileReply>> Get()
        {
            var sub = User.FindFirst(JwtClaimTypes.Subject).Value;
            return await _users.GetProfileAsync(new ProfileRequest{Sub = sub});
        }
    }
}

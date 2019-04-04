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

        [HttpGet("profile")]
        public async Task<ActionResult<RolesViewModel>> Get()
        {
            return await _client.Roles_GetAllRolesAsync(HttpContext.RequestAborted);
        }
    }
}

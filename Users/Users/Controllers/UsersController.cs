using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Queries;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersQueries _queries;

        public UsersController(IUsersQueries queries)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
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
    }
}
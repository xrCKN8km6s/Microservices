using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Queries;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUsersQueries _queries;

        public RolesController(IUsersQueries queries)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        [HttpGet("")]
        public async Task<ActionResult<RolesViewModel>> GetAllRoles()
        {
            var res = await _queries.GetRolesViewModelAsync();

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }
    }
}
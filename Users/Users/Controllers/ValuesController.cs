using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Infrastructure;
using Users.Models;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var conn = "Host=localhost;Database=Users;Username=db_user;Password=db_pass";

            var builder = new DbContextOptionsBuilder<UsersContext>().UseNpgsql(conn);

            var context = new UsersContext(builder.Options);

            var users = await context.Users.Include(u => u.UserRoles).ThenInclude(t => t.Role).ThenInclude(t => t.PermissionRoles).ToArrayAsync();

            var permisisons = users.First().UserRoles.Select(s => s.Role).SelectMany(s => s.PermissionRoles)
                .Select(s => s.Permission);

            var ps = Permission.GetAll();

            return Ok(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

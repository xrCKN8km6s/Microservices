using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Infrastructure;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController()
        {
        }

        [HttpGet("")]
        public Task<ActionResult<dynamic>> GetUserWithoutSub()
        {
            var sub = User.FindFirst(c => c.Type == JwtClaimTypes.Subject).Value;
            return GetUser(sub);
        }


        [HttpGet("{sub}")]
        public async Task<ActionResult<dynamic>> GetUser(string sub)
        {
            var conn = "Host=localhost;Database=Users;Username=db_user;Password=db_pass";

            var builder = new DbContextOptionsBuilder<UsersContext>().UseNpgsql(conn);

            var context = new UsersContext(builder.Options);

            var user = await context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(t => t.Role)
                .ThenInclude(t => t.PermissionRoles)
                .Where(w => w.IsActive)
                .FirstOrDefaultAsync(f => f.Sub == sub);

            if (user == null)
            {
                return NotFound();
            }

            return new
            {
                user.Sub,
                user.Id,
                IsGlobal = user.UserRoles.Select(s => s.Role).Any(a => a.IsGlobal),
                Permissions = user.UserRoles
                    .Select(s => s.Role)
                    .SelectMany(s => s.PermissionRoles)
                    .Select(s => s.Permission)
                    .ToArray()
            };
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Orders.Client.Contracts;

namespace BFF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersClient _client;

        public OrdersController(IOrdersClient client)
        {
            _client = client;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<OrderModel>>> Get()
        {
            var sub = User.FindFirst(JwtClaimTypes.Subject).Value;
            return Ok(await _client.GetAsync(HttpContext.RequestAborted));
        }
    }
}

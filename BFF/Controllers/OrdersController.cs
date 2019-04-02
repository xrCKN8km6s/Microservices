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

        [HttpGet("create")]
        public async Task<ActionResult> CreateOrder([FromQuery]string name)
        {
            await _client.CreateOrderAsync(name, HttpContext.RequestAborted);
            return Ok();
        }

        [HttpGet("setStatus")]
        public async Task<ActionResult> SetOrderStatus([FromQuery]long orderId, [FromQuery]int status)
        {
            await _client.UpdateOrderStatusAsync(orderId, status, HttpContext.RequestAborted);
            return Ok();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Client.Contracts;

namespace BFF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ViewOrders")]
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
            return Ok(await _client.Orders_GetAsync(HttpContext.RequestAborted));
        }

        [HttpGet("create")]
        [Authorize(Policy = "EditOrders")]
        public async Task<ActionResult> CreateOrder([FromQuery]string name)
        {
            await _client.Orders_CreateOrderAsync(name, HttpContext.RequestAborted);
            return Ok();
        }

        [HttpGet("setStatus")]
        [Authorize(Policy = "EditOrders")]
        public async Task<ActionResult> SetOrderStatus([FromQuery]long orderId, [FromQuery]int status)
        {
            await _client.Orders_UpdateOrderStatusAsync(orderId, status, HttpContext.RequestAborted);
            return Ok();
        }
    }
}

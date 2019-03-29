using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.Queries;

namespace Orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _query;

        public OrdersController(IMediator mediator, IOrderQueries query)
        {
            _mediator = mediator;
            _query = query;
        }

        [HttpGet("")]
        [Authorize(Policy = "ViewOrders")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _query.GetOrdersAsync());
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateOrder([FromQuery] CreateOrderCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet("setStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromQuery] ChangeOrderStatusCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }
    }
}

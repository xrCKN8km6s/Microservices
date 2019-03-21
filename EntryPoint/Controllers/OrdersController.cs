using EntryPoint.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EntryPoint.Application.Queries;

namespace EntryPoint.Controllers
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
        public async Task<IActionResult> About()
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

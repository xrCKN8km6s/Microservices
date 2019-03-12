using EntryPoint.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EntryPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
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

using EntryPoint.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EntryPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateOrder([FromQuery] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok();
        }

        [HttpGet("setStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromQuery] ChangeOrderStatusCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok();
        }

    }
}

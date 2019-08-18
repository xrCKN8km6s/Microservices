using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Orders.Domain.Aggregates.Order;

namespace Orders.Application.Commands
{
    public class CreateOrderCommand : IRequest<bool>
    {
        public string Name { get; set; }
    }

    [UsedImplicitly]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _repo;

        public CreateOrderCommandHandler(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.CreateNew(request.Name);

            await _repo.AddAsync(order);

            return await _repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
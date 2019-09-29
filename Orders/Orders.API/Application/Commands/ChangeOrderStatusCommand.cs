using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Orders.Domain.Aggregates.Order;

namespace Orders.API.Application.Commands
{
    public class ChangeOrderStatusCommand : IRequest<bool>
    {
        public long OrderId { get; set; }
        public int Status { get; set; }
    }

    [UsedImplicitly]
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _repo;

        public ChangeOrderStatusCommandHandler(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _repo.GetAsync(request.OrderId);

            order.SetStatusTo(request.Status);

            return await _repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
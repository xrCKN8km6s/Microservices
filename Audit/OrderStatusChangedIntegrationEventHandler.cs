using System.Threading.Tasks;
using EventBus.Abstractions;

namespace Audit
{
    public class OrderStatusChangedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedIntegrationEvent>
    {
        public Task Handle(OrderStatusChangedIntegrationEvent e)
        {
            return Task.CompletedTask;
        }
    }
}
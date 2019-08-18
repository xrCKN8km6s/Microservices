using System.Threading.Tasks;
using EventBus.Abstractions;
using Orders.Application.IntegrationEvents.Events;

namespace Orders.Application.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedIntegrationEvent>
    {
        public Task Handle(OrderStatusChangedIntegrationEvent e)
        {
            return Task.CompletedTask;
        }
    }
}
using System.Threading.Tasks;
using EventBus;
using Orders.API.Application.IntegrationEvents.Events;

namespace Orders.API.Application.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedIntegrationEvent>
    {
        public Task Handle(OrderStatusChangedIntegrationEvent e)
        {
            return Task.CompletedTask;
        }
    }
}
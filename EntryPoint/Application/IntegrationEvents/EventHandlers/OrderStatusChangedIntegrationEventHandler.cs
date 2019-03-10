using System.Threading.Tasks;
using EntryPoint.Application.IntegrationEvents.Events;
using EventBus.Abstractions;

namespace EntryPoint.Application.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedIntegrationEvent>
    {
        public Task Handle(OrderStatusChangedIntegrationEvent e)
        {
            return Task.CompletedTask;
        }
    }
}
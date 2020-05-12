using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Orders.API.Application.IntegrationEvents;
using Orders.API.Application.IntegrationEvents.Events;
using Orders.Domain.Events;

namespace Orders.API.Application.DomainEventHandlers
{
    public class OrderStatusChangedDomainEventHandler : INotificationHandler<OrderStatusChangedDomainEvent>
    {
        private readonly IOrderingIntegrationEventService _eventLogService;

        public OrderStatusChangedDomainEventHandler(IOrderingIntegrationEventService eventLogService)
        {
            _eventLogService = eventLogService;
        }

        public async Task Handle(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent =
                new OrderStatusChangedIntegrationEvent(notification.Id, notification.OldStatus, notification.NewStatus);

            await _eventLogService.SaveEventAsync(integrationEvent);
        }
    }
}
using EntryPoint.Application.IntegrationEvents;
using EntryPoint.Application.IntegrationEvents.Events;
using EntryPoint.Domain.Events;
using JetBrains.Annotations;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EntryPoint.Application.DomainEventHandlers
{
    [UsedImplicitly]
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
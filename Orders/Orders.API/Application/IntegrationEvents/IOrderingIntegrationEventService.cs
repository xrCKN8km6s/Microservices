using EventBus;

namespace Orders.API.Application.IntegrationEvents;

public interface IOrderingIntegrationEventService
{
    Task PublishEventsAsync(Guid transactionId);
    Task SaveEventAsync(IIntegrationEvent evt);
}

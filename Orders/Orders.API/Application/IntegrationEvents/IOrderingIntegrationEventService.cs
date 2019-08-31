using System.Threading.Tasks;
using EventBus.Events;

namespace Orders.Application.IntegrationEvents
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsAsync();

        Task SaveEventAsync(IntegrationEvent evt);
    }
}
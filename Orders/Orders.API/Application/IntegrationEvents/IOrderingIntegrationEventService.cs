using System.Threading.Tasks;
using EventBus;

namespace Orders.API.Application.IntegrationEvents
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsAsync();

        Task SaveEventAsync(IntegrationEvent evt);
    }
}
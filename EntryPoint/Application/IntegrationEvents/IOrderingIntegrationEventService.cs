using EventBus.Events;
using System.Threading.Tasks;

namespace EntryPoint.Application.IntegrationEvents
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsAsync();

        Task SaveEventAsync(IntegrationEvent evt);
    }
}
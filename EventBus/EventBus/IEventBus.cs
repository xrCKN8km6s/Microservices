using System;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IEventBus
    {
        Task Publish(IIntegrationEvent e);

        Task Publish(Guid eventId, string eventName, string content);

        Task Subscribe(Action<EventSubscriptions> setupSubscriptions);

        void Unsubscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}

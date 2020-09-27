using System;

namespace EventBus
{
    public interface IEventBus
    {
        void Publish(IIntegrationEvent e);

        void Publish(Guid eventId, string eventName, string content);

        void Subscribe(Action<EventSubscriptions> setupSubscriptions);

        void Unsubscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}

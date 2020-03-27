using System;

namespace EventBus
{
    public interface IEventBus
    {
        void Publish(IIntegrationEvent e);

        void Publish(Guid eventId, string eventName, string content);

        void Subscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
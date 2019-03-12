using System;
using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent e);

        void Publish(Guid eventId, string eventName, string content);

        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void Consume();
    }
}
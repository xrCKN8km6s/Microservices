using EventBus.Abstractions;
using EventBus.Events;
using System;
using System.Collections.Generic;

namespace EventBus
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }

        bool HasSubscriptionsForEvent(string eventName);

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void Clear();

        IReadOnlyCollection<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

        IReadOnlyCollection<SubscriptionInfo> GetHandlersForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        string GetEventKey<T>();
    }
}
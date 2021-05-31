using System;
using System.Collections.Generic;

namespace EventBus
{
    public class EventSubscriptions
    {
        public class EventSubscription
        {
            public Type EventType { get; set; }
            public Type HandlerType { get; set; }

            internal EventSubscription() {}
        }

        private readonly List<EventSubscription> _subscriptions = new();

        public IReadOnlyCollection<EventSubscription> Subscriptions => _subscriptions.AsReadOnly();

        public EventSubscriptions Add<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            _subscriptions.Add(new EventSubscription { EventType = typeof(T), HandlerType = typeof(TH) });
            return this;
        }
    }
}

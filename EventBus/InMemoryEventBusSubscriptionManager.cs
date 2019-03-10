using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public bool IsEmpty => _handlers.Count == 0;

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var key = GetEventKey<T>();
            var handlerType = typeof(TH);

            if (!HasSubscriptionsForEvent(key))
            {
                _handlers.Add(key, new List<SubscriptionInfo>());
            }

            if (_handlers[key].Any(a => a.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler {handlerType.Name} is already registered for {key}.");
            }

            _handlers[key].Add(SubscriptionInfo.Typed(handlerType));

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var key = GetEventKey<T>();

            var handlerType = typeof(TH);

            var subscriptionToRemove = HasSubscriptionsForEvent(key)
                ? _handlers[key].SingleOrDefault(s => s.HandlerType == handlerType)
                : null;

            if (subscriptionToRemove == null)
            {
                return;
            }

            _handlers[key].Remove(subscriptionToRemove);

            if (_handlers[key].Count == 0)
            {
                return;
            }

            _handlers.Remove(key);

            var eventTypeToRemove = _eventTypes.SingleOrDefault(s => s.Name == key);
            if (eventTypeToRemove != null)
            {
                _eventTypes.Remove(eventTypeToRemove);

                OnEventRemoved?.Invoke(this, key);
            }
        }

        public IReadOnlyCollection<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();

            return GetHandlersForEvent(key);
        }

        public IReadOnlyCollection<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();

            return HasSubscriptionsForEvent(key);
        }

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(s => s.Name == eventName);

        public string GetEventKey<T>() => typeof(T).Name;

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}
namespace EventBus;

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

    public void AddSubscription<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        AddSubscription(typeof(T), typeof(TH));
    }

    public void AddSubscription(Type eventType, Type eventHandlerType)
    {
        if (eventType == null)
        {
            throw new ArgumentNullException(nameof(eventType));
        }

        if (eventHandlerType == null)
        {
            throw new ArgumentNullException(nameof(eventHandlerType));
        }

        var key = GetEventKey(eventType);

        if (!HasSubscriptionsForEvent(key))
        {
            _handlers.Add(key, new List<SubscriptionInfo>());
        }

        if (_handlers[key].Any(a => a.HandlerType == eventHandlerType))
        {
            throw new ArgumentException($"Handler {eventHandlerType.Name} is already registered for {key}.");
        }

        _handlers[key].Add(SubscriptionInfo.Typed(eventHandlerType));

        if (!_eventTypes.Contains(eventType))
        {
            _eventTypes.Add(eventType);
        }
    }

    public void RemoveSubscription<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
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

    public IReadOnlyCollection<SubscriptionInfo> GetHandlersForEvent<T>() where T : IIntegrationEvent
    {
        var key = GetEventKey<T>();

        return GetHandlersForEvent(key);
    }

    public IReadOnlyCollection<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent
    {
        var key = GetEventKey<T>();

        return HasSubscriptionsForEvent(key);
    }

    public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(s => s.Name == eventName);

    public string GetEventKey<T>() => GetEventKey(typeof(T));

    public string GetEventKey(Type eventType)
    {
        return eventType?.Name ?? throw new ArgumentNullException(nameof(eventType));
    }

    public void Clear()
    {
        _handlers.Clear();
    }
}

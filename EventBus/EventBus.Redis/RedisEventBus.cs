using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EventBus.Redis;

public class RedisEventBus : IEventBus, IDisposable
{
    private readonly ILogger<RedisEventBus> _logger;
    private readonly IEventBusSubscriptionManager _subManager;
    private readonly IEventBusSerializer _serializer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRedisStreamsManager _manager;

    public RedisEventBus(
        ILogger<RedisEventBus> logger,
        IEventBusSubscriptionManager subManager,
        IEventBusSerializer serializer,
        IRedisStreamsManager manager,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subManager = subManager ?? throw new ArgumentNullException(nameof(subManager));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public async Task Publish(IIntegrationEvent e)
    {
        if (e == null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        var eventName = e.GetType().Name;
        var body = _serializer.Serialize(e);

        await Publish(e.Id, eventName, body);
    }

    public async Task Publish(Guid eventId, string eventName, string content)
    {
        var body = Encoding.UTF8.GetBytes(content);

        await Publish(eventId, eventName, body);
    }

    private async Task Publish(Guid eventId, string eventName, byte[] body)
    {
        await _manager.PublishEvent(eventId.ToString(), eventName, body);
    }

    public async Task Subscribe(Action<EventSubscriptions> setupSubscriptions)
    {
        _ = setupSubscriptions ?? throw new ArgumentNullException(nameof(setupSubscriptions));

        var subs = new EventSubscriptions();
        setupSubscriptions(subs);

        foreach (var sub in subs.Subscriptions)
        {
            var eventName = sub.EventType.Name;

            await _manager.CreateConsumerGroup(eventName);

            _logger.LogDebug("Subscribing to event {EventName} with {EventHandler}", eventName, sub.HandlerType.GetGenericTypeName());

            _subManager.AddSubscription(sub.EventType, sub.HandlerType);
        }

        //"OrderStatusChangedIntegrationEvent"
        var eventNames = subs.Subscriptions.Select(s => s.EventType.Name).ToArray();

        _manager.Start(eventNames, async (message) => { await ProcessEvent(message); });
    }

    private async Task ProcessEvent(StreamsMessage message)
    {
        if (_subManager.HasSubscriptionsForEvent(message.EventName))
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var subscriptions = _subManager.GetHandlersForEvent(message.EventName);
            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                if (handler == null)
                {
                    continue;
                }

                var eventType = _subManager.GetEventTypeByName(message.EventName);
                var integrationEvent = _serializer.Deserialize(message.Content, eventType);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });

                _logger.LogTrace("Processed {EventName} {EventId}", message.EventName, message.MessageId);
            }
        }
    }

    public void Unsubscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = _subManager.GetEventKey<T>();

        _manager.DeleteConsumerGroup(eventName);

        _logger.LogDebug("Unsubscribing from event {EventName}", eventName);

        _subManager.RemoveSubscription<T, TH>();
    }

    public void Dispose()
    {
        _manager.Stop();
        _subManager.Clear();
    }
}

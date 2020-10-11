using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Redis
{
    public class RedisEventBus : IEventBus, IDisposable
    {
        private readonly ILogger<RedisEventBus> _logger;
        private readonly IDatabase _db;
        private readonly IEventBusSubscriptionManager _subManager;
        private readonly IEventBusSerializer _serializer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRedisStreamsConsumer _consumer;

        public RedisEventBus(
            ILogger<RedisEventBus> logger,
            IDatabase db,
            IEventBusSubscriptionManager subManager,
            IEventBusSerializer serializer,
            IRedisStreamsConsumer consumer,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _subManager = subManager ?? throw new ArgumentNullException(nameof(subManager));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public void Publish(IIntegrationEvent e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var eventName = e.GetType().Name;
            var body = _serializer.Serialize(e);

            Publish(e.Id, eventName, body);
        }

        public void Publish(Guid eventId, string eventName, string content)
        {
            var body = Encoding.UTF8.GetBytes(content);

            Publish(eventId, eventName, body);
        }

        private void Publish(Guid eventId, string eventName, byte[] body)
        {
            _db.StreamAdd(eventName, new[] { 
                new NameValueEntry("id", eventId.ToString()),
                new NameValueEntry("content", body)
            });
        }

        public void Subscribe(Action<EventSubscriptions> setupSubscriptions)
        {
            _ = setupSubscriptions ?? throw new ArgumentNullException(nameof(setupSubscriptions));

            var subs = new EventSubscriptions();
            setupSubscriptions(subs);

            foreach (var sub in subs.Subscriptions)
            {
                var eventName = sub.EventType.Name;

                //rewrite to check if group exists
                try
                {
                    _db.StreamCreateConsumerGroup(eventName, "Orders", StreamPosition.Beginning);
                }
                catch (RedisServerException) {}

                _logger.LogDebug("Subscribing to event {EventName} with {EventHandler}", eventName, sub.HandlerType.GetGenericTypeName());

                _subManager.AddSubscription(sub.EventType, sub.HandlerType);
            }

            //"OrderStatusChangedIntegrationEvent"
            var eventNames = subs.Subscriptions.Select(s => s.EventType.Name).ToArray();

            _consumer.Start(eventNames, async (eventName, streamEntry) => { await ProcessEvent(eventName, streamEntry); });
        }

        private async Task ProcessEvent(string name, StreamEntry item)
        {
            var (id, message) = (item.Id, item["content"]);

            if (_subManager.HasSubscriptionsForEvent(name))
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var subscriptions = _subManager.GetHandlersForEvent(name);
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                    if (handler == null)
                    {
                        continue;
                    }

                    var eventType = _subManager.GetEventTypeByName(name);
                    var integrationEvent = _serializer.Deserialize(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });

                    _logger.LogTrace("Processed {EventName} {EventId}", name, id);
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subManager.GetEventKey<T>();

            _db.StreamDeleteConsumerGroup(eventName, "Orders");

            _logger.LogDebug("Unsubscribing from event {EventName}", eventName);

            _subManager.RemoveSubscription<T, TH>();
        }

        public void Dispose()
        {
            _consumer.Stop();
            _subManager.Clear();
        }
    }
}

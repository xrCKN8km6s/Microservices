using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IRabbitMQConnection _connection;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEventBusSubscriptionManager _subManager;
        private readonly IEventBusSerializer _serializer;
        private readonly RabbitMQEventBusOptions _options;
        private IModel _consumerChannel;

        public RabbitMQEventBus(
            IRabbitMQConnection connection,
            ILogger<RabbitMQEventBus> logger,
            IServiceScopeFactory serviceScopeFactory,
            IEventBusSubscriptionManager subManager,
            IEventBusSerializer serializer,
            RabbitMQEventBusOptions options)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _subManager = subManager ?? throw new ArgumentNullException(nameof(subManager));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _subManager.OnEventRemoved += SubsManager_OnEventRemoved;
            _consumerChannel = CreateConsumerChannel();
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(
                    queue: _options.QueueName,
                    exchange: _options.ExchangeName,
                    routingKey: eventName);

                if (_subManager.IsEmpty)
                {
                    _consumerChannel.Close();
                }
            }
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
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", eventId);

            var policy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.PublishRetryAttempts, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (exception, span) =>
                    {
                        _logger.LogWarning(exception,
                            "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", eventId,
                            $"{span.TotalSeconds:n1}", exception.Message);
                    });

            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: _options.ExchangeName,
                    type: ExchangeType.Direct,
                    durable: true);

                var attempt = 0;

                policy.Execute(() =>
                {
                    attempt++;

                    var properties = channel.CreateBasicProperties();

                    properties.DeliveryMode = 2;
                    properties.Headers = new Dictionary<string, object>
                    {
                        ["x-publish-attempt"] = attempt
                    };

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", eventId);

                    channel.BasicPublish(_options.ExchangeName, eventName, true, properties, body);
                });
            }
        }

        public void Subscribe(Action<EventSubscriptions> setupSubscriptions)
        {
            _ = setupSubscriptions ?? throw new ArgumentNullException(nameof(setupSubscriptions));

            var subs = new EventSubscriptions();
            setupSubscriptions(subs);

            foreach (var eventSubscription in subs.Subscriptions)
            {
                var eventName = _subManager.GetEventKey(eventSubscription.EventType);
                DoInternalSubscription(eventName);

                _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, eventSubscription.HandlerType.GetGenericTypeName());

                _subManager.AddSubscription(eventSubscription.EventType, eventSubscription.HandlerType);
            }

            StartBasicConsume();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_connection.IsConnected)
                {
                    _connection.TryConnect();
                }

                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _options.QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.QueueBind(
                        queue: _options.QueueName,
                        exchange: _options.ExchangeName,
                        routingKey: eventName);
                }
            }
        }

        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchange: _options.ExchangeName,
                                    type: ExchangeType.Direct,
                                    durable: true);

            channel.QueueDeclare(queue: _options.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _options.QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
                _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
                _consumerChannel.BasicNack(eventArgs.DeliveryTag, false, true);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //_consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscriptions = _subManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null)
                        {
                            continue;
                        }

                        var eventType = _subManager.GetEventTypeByName(eventName);
                        var integrationEvent = _serializer.Deserialize(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await ((Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent }))
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subManager.Clear();
        }
    }
}

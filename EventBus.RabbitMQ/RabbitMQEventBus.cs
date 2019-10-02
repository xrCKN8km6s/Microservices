using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private const string EXCHANGE_NAME = "microservice_event_bus";

        private readonly IRabbitMQConnection _connection;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEventBusSubscriptionManager _subManager;
        private readonly string _queueName;
        private readonly int _retryCount;
        private IModel _consumerChannel;

        public RabbitMQEventBus(IRabbitMQConnection connection, ILogger<RabbitMQEventBus> logger,
            IServiceScopeFactory serviceScopeFactory, IEventBusSubscriptionManager subManager, string queueName = "",
            int retryCount = 5)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _subManager = subManager ?? throw new ArgumentNullException(nameof(subManager));
            _queueName = queueName;
            _retryCount = retryCount;
        }

        public void Publish(IntegrationEvent e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var eventName = e.GetType().Name;
            var content = JsonConvert.SerializeObject(e);

            PublishInternal(e.Id, eventName, content);
        }

        public void Publish(Guid eventId, string eventName, string content)
        {
            PublishInternal(eventId, eventName, content);
        }

        private void PublishInternal(Guid eventId, string eventName, string content)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (exception, span) =>
                    {
                        _logger.LogWarning(exception,
                            "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", eventId,
                            $"{span.TotalSeconds:n1}", exception.Message);
                    });

            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);

                var body = Encoding.UTF8.GetBytes(content);

                var attempt = 0;

                policy.Execute(() =>
                {
                    attempt++;

                    //Can't set InstantHandleAttribute
                    // ReSharper disable AccessToDisposedClosure

                    var properties = channel.CreateBasicProperties();

                    properties.DeliveryMode = 2;
                    properties.Headers = new Dictionary<string, object>
                    {
                        ["x-publish-attempt"] = attempt
                    };

                    channel.BasicPublish(EXCHANGE_NAME, eventName, true, properties, body);

                    // ReSharper restore AccessToDisposedClosure
                });
            }
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subManager.GetEventKey<T>();

            if (!_subManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_connection.IsConnected)
                {
                    _connection.TryConnect();
                }

                using (var channel = _connection.CreateModel())
                {
                    channel.QueueBind(_queueName, EXCHANGE_NAME, eventName, null);
                }
            }

            _logger.LogInformation("Subscribing to {Event} with {EventHandler}", eventName,
                typeof(TH).GetGenericTypeName());

            _subManager.AddSubscription<T, TH>();
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from {Event} with {EventHandler}.", eventName,
                typeof(TH).GetGenericTypeName());

            _subManager.RemoveSubscription<T, TH>();
        }

        public void Consume()
        {
            _consumerChannel = CreateConsumerChannel();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(EXCHANGE_NAME,
                "direct",
                true);

            channel.QueueDeclare(_queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message).ConfigureAwait(false);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(queue: _queueName, false, consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
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
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await ((Task) concreteType.GetMethod("Handle").Invoke(handler, new[] {integrationEvent}))
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
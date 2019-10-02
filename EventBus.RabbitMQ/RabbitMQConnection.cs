using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public sealed class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private readonly int _retryCount;
        private IConnection _connection;
        private bool _isDisposed;
        private readonly object _syncRoot = new object();
        public bool IsConnected => _connection != null && _connection.IsOpen && !_isDisposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connection.");
            }

            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ is trying to connect.");

            lock (_syncRoot)
            {
                var policy = Policy
                    .Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                        (exception, span) =>
                        {
                            _logger.LogWarning(exception, "RabbitMq could not connect after {Times} ({Message})",
                                $"{span.TotalSeconds:N1}", exception.Message);
                        });

                policy.Execute(() => { _connection = _connectionFactory.CreateConnection(); });

                if (IsConnected)
                {
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _connection.ConnectionShutdown += OnConnectionShutdown;

                    _logger.LogInformation("RabbitMQ successfully connected to {HostName}.", _connection.Endpoint.HostName);

                    return true;
                }

                _logger.LogCritical("RabbitMQ could not connect.");
                return false;
            }
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_isDisposed)
            {
                return;
            }

            _logger.LogWarning("RabbitMQ connection is on shutdown ({@Reason}). Reconnecting...", e.ReplyText);

            TryConnect();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_isDisposed)
            {
                return;
            }

            _logger.LogWarning("RabbitMQ connection is blocked ({Reason}). Reconnecting...", e.Reason);

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_isDisposed)
            {
                return;
            }

            _logger.LogWarning(e.Exception, "RabbitMQ connection throw exception ({Message}). Reconnecting...", e.Exception.Message);

            TryConnect();
        }


        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                _connection?.Dispose();
                _isDisposed = true;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.ToString());
            }
        }
    }
}
using EventBus;
using EventBus.AspNetCore;
using EventBus.RabbitMQ;
using EventBus.RabbitMQ.AspNetCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusBuilderExtensions
    {
        public static EventBusBuilder UseRabbitMQ(this EventBusBuilder builder, Action<RabbitMQOptions> setupOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupOptions is null)
            {
                throw new ArgumentNullException(nameof(setupOptions));
            }

            var options = new RabbitMQOptions();
            setupOptions(options);

            builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var connectionLogger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
                var factory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    VirtualHost = options.VirtualHost,
                    UserName = options.UserName,
                    Password = options.Password,
                    DispatchConsumersAsync = true
                };
                var connection = new RabbitMQConnection(factory, connectionLogger, options.ConnectRetryAttempts);

                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var subManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var serialzier = sp.GetRequiredService<IEventBusSerializer>();

                return new RabbitMQEventBus(connection, logger, serviceScopeFactory, subManager, serialzier, options.ExchangeName, options.QueueName, options.PublishRetryAttempts);
            });

            return builder;
        }
    }
}

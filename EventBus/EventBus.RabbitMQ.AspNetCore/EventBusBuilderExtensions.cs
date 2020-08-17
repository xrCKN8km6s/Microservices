using EventBus;
using EventBus.AspNetCore;
using EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusBuilderExtensions
    {
        public static EventBusBuilder UseRabbitMQ(this EventBusBuilder builder,
            Action<RabbitMQConnectionOptions> setupConnectionOptions,
            Action<RabbitMQEventBusOptions> setupEventBusOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupConnectionOptions is null)
            {
                throw new ArgumentNullException(nameof(setupConnectionOptions));
            }

            if (setupEventBusOptions is null)
            {
                throw new ArgumentNullException(nameof(setupEventBusOptions));
            }

            var connectionOptions = new RabbitMQConnectionOptions();
            setupConnectionOptions(connectionOptions);

            var eventBusOptions = new RabbitMQEventBusOptions();
            setupEventBusOptions(eventBusOptions);

            return UseRabbitMQ(builder, connectionOptions, eventBusOptions);
        }

        public static EventBusBuilder UseRabbitMQ(this EventBusBuilder builder,
            RabbitMQConnectionOptions connectionOptions,
            RabbitMQEventBusOptions eventBusOptions)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (connectionOptions is null)
            {
                throw new ArgumentNullException(nameof(connectionOptions));
            }

            if (eventBusOptions is null)
            {
                throw new ArgumentNullException(nameof(eventBusOptions));
            }

            builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var connectionLogger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
                var factory = new ConnectionFactory
                {
                    HostName = connectionOptions.HostName,
                    VirtualHost = connectionOptions.VirtualHost,
                    UserName = connectionOptions.UserName,
                    Password = connectionOptions.Password,
                    DispatchConsumersAsync = true
                };
                var connection = new RabbitMQConnection(factory, connectionLogger, connectionOptions.ConnectRetryAttempts);

                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var subManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var serialzier = sp.GetRequiredService<IEventBusSerializer>();

                return new RabbitMQEventBus(connection, logger, serviceScopeFactory, subManager, serialzier, eventBusOptions);
            });

            return builder;
        }
    }
}

using EventBus;
using EventBus.AspNetCore;
using Microsoft.Extensions.Logging;
using System;
using EventBus.Redis;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusBuilderExtensions
    {
        public static EventBusBuilder UseRedis(this EventBusBuilder builder,
            Action<RedisEventBusOptions> setupOptions)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (setupOptions == null) throw new ArgumentNullException(nameof(setupOptions));

            var connectionOptions = new RedisEventBusOptions();
            setupOptions(connectionOptions);

            return UseRedis(builder, connectionOptions);
        }

        public static EventBusBuilder UseRedis(this EventBusBuilder builder,
            RedisEventBusOptions options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (options == null) throw new ArgumentNullException(nameof(options));

            builder.Services.AddSingleton(sp =>
            {
                var connection = ConnectionMultiplexer.Connect(options.Configuration);
                return connection.GetDatabase();
            });

            builder.Services.AddSingleton<IEventBus>(sp =>
            {
                var db = sp.GetRequiredService<IDatabase>();

                var managerLogger = sp.GetRequiredService<ILogger<RedisStreamsManager>>();

                var consumer = new RedisStreamsManager(db, options.ConsumerGroupName, options.ConsumerName, options.BatchPerGroupSize, managerLogger);

                var logger = sp.GetRequiredService<ILogger<RedisEventBus>>();
                var subManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var serializer = sp.GetRequiredService<IEventBusSerializer>();

                var bus = new RedisEventBus(
                    logger,
                    subManager,
                    serializer,
                    consumer,
                    serviceScopeFactory);
                return bus;
            });

            return builder;
        }
    }
}

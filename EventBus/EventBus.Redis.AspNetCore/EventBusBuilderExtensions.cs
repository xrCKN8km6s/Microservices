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

            builder.Services.AddSingleton<IEventBus>(sp =>
            {
                var connection = ConnectionMultiplexer.Connect(options.Configuration);
                var db = connection.GetDatabase();

                var consumer = new RedisStreamsConsumer(db, options.ConsumerGroupName, options.ConsumerName, options.BatchPerGroupSize);

                var logger = sp.GetService<ILogger<RedisEventBus>>();
                var subManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var serializer = sp.GetRequiredService<IEventBusSerializer>();

                var bus = new RedisEventBus(
                    logger,
                    db,
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

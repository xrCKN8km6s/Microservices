using EventBus;
using EventBus.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusBuilderExtensions
    {
        public static EventBusBuilder UseInMemorySubscriptionManager(this EventBusBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
            return builder;
        }

        public static EventBusBuilder UseJsonNetSerializer(this EventBusBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton<IEventBusSerializer, JsonNetEventBusSerializer>();
            return builder;
        }
    }
}

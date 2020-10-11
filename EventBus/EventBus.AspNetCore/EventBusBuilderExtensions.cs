using EventBus;
using EventBus.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusBuilderExtensions
    {
        public static EventBusBuilder UseSubscriptionManager<T>(this EventBusBuilder builder) where T : class, IEventBusSubscriptionManager
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IEventBusSubscriptionManager, T>();
            return builder;
        }

        public static EventBusBuilder UseInMemorySubscriptionManager(this EventBusBuilder builder)
        {
            return builder.UseSubscriptionManager<InMemoryEventBusSubscriptionManager>();
        }

        public static EventBusBuilder UseSerializer<T>(this EventBusBuilder builder) where T : class, IEventBusSerializer
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IEventBusSerializer, JsonNetEventBusSerializer>();
            return builder;
        }

        public static EventBusBuilder UseJsonNetSerializer(this EventBusBuilder builder)
        {
            return builder.UseSerializer<JsonNetEventBusSerializer>();
        }
    }
}

using EventBus.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, Action<EventBusBuilder> build)
        {
            var builder = new EventBusBuilder(services);
            build?.Invoke(builder);
            return services;
        }
    }
}

using EventBus.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<EventBusBuilder> build)
        {
            var builder = new EventBusBuilder(services);
            build?.Invoke(builder);
            return services;
        }
    }
}

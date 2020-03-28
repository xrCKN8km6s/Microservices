using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.AspNetCore
{
    public class EventBusBuilder
    {
        public IServiceCollection Services { get; }

        public EventBusBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}

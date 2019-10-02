using System;
using Microsoft.Extensions.DependencyInjection;

namespace BFF
{
    public static class ClientExtensions
    {
        public static void AddClient<TI, TC>(this IServiceCollection services, string baseAddress)
            where TI : class where TC : class, TI
        {
            services.AddHttpClient<TI, TC>(c => { c.BaseAddress = new Uri(baseAddress); })
                .AddHttpMessageHandler<BearerTokenDelegatingHandler>();
        }
    }
}
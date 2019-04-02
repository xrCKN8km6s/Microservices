using System;
using Microsoft.Extensions.DependencyInjection;

namespace BFF
{
    public static class ClientExtensions
    {
        public static void AddClient<TI, TC>(this IServiceCollection services, string baseAddress, ClientConfiguration config)
            where TI : class where TC : class, TI
        {
            services.AddHttpClient<TI, TC>(c => { c.BaseAddress = new Uri(baseAddress); })
                .AddHttpMessageHandler(sp =>
                {
                    var tokenAccessor = sp.GetRequiredService<ITokenAccessor>();
                    return new BearerTokenDelegatingHandler(tokenAccessor, config);
                });
        }
    }
}
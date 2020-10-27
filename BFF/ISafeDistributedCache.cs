using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BFF
{
    public interface ISafeDistributedCache
    {
        Task<string> GetStringAsync(string key, CancellationToken token);

        Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options,
            CancellationToken token = default);

        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default);

        Task<byte[]> GetAsync(string key, CancellationToken token = default);
    }

    public class SafeDistributedCache : ISafeDistributedCache
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<SafeDistributedCache> _logger;

        public SafeDistributedCache(IDistributedCache cache, ILogger<SafeDistributedCache> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetStringAsync(string key, CancellationToken token)
        {
            try
            {
                return await _cache.GetStringAsync(key, token);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception while accessing cache server.");
                return null;
            }
        }

        public async Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options,
            CancellationToken token)
        {
            try
            {
                await _cache.SetStringAsync(key, value, options, token);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception while accessing cache server.");
            }
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            try
            {
                await _cache.SetAsync(key, value, options, token);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception while accessing cache server.");
            }
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            try
            {
                return await _cache.GetAsync(key, token);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Exception while accessing cache server.");
                return null;
            }
        }
    }
}

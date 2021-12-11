using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace BFF;

public interface ITokenAccessor
{
    Task<string> GetAccessToken(CancellationToken cancellationToken);
}

public class TokenAccessor : ITokenAccessor
{
    private readonly HttpClient _client;
    private readonly TokenAccessorOptions _config;
    private readonly ISafeDistributedCache _cache;
    private readonly ILogger<TokenAccessor> _logger;

    public TokenAccessor(
        HttpClient client,
        IOptions<TokenAccessorOptions> config,
        ISafeDistributedCache cache,
        ILogger<TokenAccessor> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GetAccessToken(CancellationToken cancellationToken)
    {
        var cacheKey = $"access_token:{_config.ClientId}";

        var cachedToken = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedToken))
        {
            return cachedToken;
        }

        var token = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            ClientId = _config.ClientId,
            ClientSecret = _config.ClientSecret,
            Scope = _config.Scopes
        }, cancellationToken);

        if (token.IsError)
        {
            _logger.LogError(token.Exception, "Exception while retrieving access token for {client}.",
                _config.ClientId);

            throw new Exception("Error while retrieving access token.", token.Exception);
        }

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn * 0.95)
        };

        await _cache.SetStringAsync(cacheKey, token.AccessToken, cacheOptions, cancellationToken);

        return token.AccessToken;
    }
}

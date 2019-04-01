using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Orders
{
    public interface ITokenAccessor
    {
        Task<string> GetAccessToken(ClientConfiguration config, CancellationToken cancellationToken);
    }

    //TODO: logging/error handling
    public class TokenAccessor : ITokenAccessor
    {
        private readonly HttpClient _client;

        public TokenAccessor(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetAccessToken(ClientConfiguration config, CancellationToken cancellationToken)
        {
            var token = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret,
                Scope = config.Scope
            }, cancellationToken);

            return token.AccessToken;
        }
    }
}
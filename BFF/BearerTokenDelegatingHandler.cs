using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BFF
{
    //TODO: logging/error handling
    public class BearerTokenDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenAccessor _tokenAccessor;
        private readonly ClientConfiguration _config;

        public BearerTokenDelegatingHandler(ITokenAccessor tokenAccessor, ClientConfiguration config)
        {
            _tokenAccessor = tokenAccessor;
            _config = config;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenAccessor.GetAccessToken(_config, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
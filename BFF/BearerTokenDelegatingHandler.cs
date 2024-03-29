using IdentityModel.Client;

namespace BFF;

public class BearerTokenDelegatingHandler : DelegatingHandler
{
    private readonly ITokenAccessor _tokenAccessor;

    public BearerTokenDelegatingHandler(ITokenAccessor tokenAccessor)
    {
        _tokenAccessor = tokenAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenAccessor.GetAccessToken(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new Exception("Error while retrieving access token.");
        }

        request.SetBearerToken(token);
        return await base.SendAsync(request, cancellationToken);
    }
}

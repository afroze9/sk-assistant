using Azure.Core;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Assistant.Desktop.Services;

public class GraphClientFactory : IGraphClientFactory
{
    private readonly GraphServiceClient _graphClient;

    public GraphClientFactory(IAuthService authService)
    {
        TokenCredential tokenCredential = DelegatedTokenCredential.Create(
            getToken: (_, _) =>
            {
                AuthenticationResult? authResult =
                    authService.AuthResult ?? authService.SignInUserAsync().GetAwaiter().GetResult();

                return authResult != null
                    ? new AccessToken(authResult.AccessToken, authResult.ExpiresOn)
                    : new AccessToken(string.Empty, DateTimeOffset.UtcNow);
            },
            getTokenAsync: async (_, _) =>
            {
                AuthenticationResult? authResult = authService.AuthResult ?? await authService.SignInUserAsync();
                return authResult != null
                    ? new AccessToken(authResult.AccessToken, authResult.ExpiresOn)
                    : new AccessToken(string.Empty, DateTimeOffset.UtcNow);
            });

        _graphClient = new GraphServiceClient(tokenCredential);
    }

    public GraphServiceClient GetGraphClient()
    {
        return _graphClient;
    }
}

public interface IGraphClientFactory
{
    GraphServiceClient GetGraphClient();
}
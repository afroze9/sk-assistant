using Assistant.Desktop.Configuration;

using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Assistant.Desktop.Services;

public class AuthService : IAuthService
{
    private readonly IdentityOptions _identityOptions;

    private readonly IPublicClientApplication _publicClientApplication;

    public AuthenticationResult? AuthResult { get; set; }

    public AuthService(IOptions<IdentityOptions> identityOptions)
    {
        _identityOptions = identityOptions.Value;
        _publicClientApplication = PublicClientApplicationBuilder
            .Create(_identityOptions.ClientId)
            .WithClientId(_identityOptions.ClientId)
            .WithTenantId(_identityOptions.TenantId)
            .WithRedirectUri(_identityOptions.RedirectUri)
            .Build();
        
        TokenCacheHelper.EnableSerialization(_publicClientApplication.UserTokenCache);
    }

    public async Task<AuthenticationResult?> SignInUserAsync(CancellationToken cancellationToken = default)
    {
        AuthenticationResult? result;

        try
        {
            IEnumerable<IAccount>? accounts = await _publicClientApplication.GetAccountsAsync();
            IAccount? account = accounts.FirstOrDefault();

            result = await _publicClientApplication
                .AcquireTokenSilent(_identityOptions.Scopes, account)
                .ExecuteAsync(cancellationToken);
        }
        catch (MsalUiRequiredException)
        {
            // Silent login failed, need to prompt the user for credentials
            try
            {
                result = await _publicClientApplication
                    .AcquireTokenInteractive(_identityOptions.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        if (result is not null)
        {
            AuthResult = result;
        }

        return result;
    }
}

public interface IAuthService
{
    AuthenticationResult? AuthResult { get; set; }

    Task<AuthenticationResult?> SignInUserAsync(CancellationToken cancellationToken = default);
}
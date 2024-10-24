﻿using Azure.Core;

using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace Assistant.Desktop.Services;

public class GraphService : IGraphService
{
    private readonly GraphServiceClient _graphClient;

    public GraphService(IAuthService authService)
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

    public async Task<User?> GetMeAsync()
    {
        return await _graphClient.Me.GetAsync();
    }
}

public interface IGraphService
{
    Task<User?> GetMeAsync();
}
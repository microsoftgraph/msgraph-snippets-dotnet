// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace SdkSnippets.Snippets;

/// <summary>
/// Implements a token provider for integrated Windows authentication.
/// </summary>
// <IntegratedWindowsTokenProviderSnippet>
public class IntegratedWindowsTokenProvider : IAccessTokenProvider
{
    private readonly IPublicClientApplication publicClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegratedWindowsTokenProvider"/> class.
    /// </summary>
    /// <param name="clientId">The client ID from the app registration in Azure.</param>
    /// <param name="tenantId">The tenant ID from the app registration in Azure.</param>
    public IntegratedWindowsTokenProvider(string clientId, string tenantId)
    {
        // From MSAL (Microsoft.Identity.Client)
        publicClient = PublicClientApplicationBuilder
            .Create(clientId)
            .WithTenantId(tenantId)
            .Build();

        AllowedHostsValidator = new AllowedHostsValidator();
    }

    /// <summary>
    /// Gets an <see cref="AllowedHostsValidator"/> that validates if the
    /// target host of a request is allowed for authentication.
    /// </summary>
    public AllowedHostsValidator AllowedHostsValidator { get; }

    /// <inheritdoc/>
    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        var scopes = new[] { "User.Read" };
        var result = await publicClient
            .AcquireTokenByIntegratedWindowsAuth(scopes)
            .ExecuteAsync(cancellationToken);
        return result.AccessToken;
    }
}
// </IntegratedWindowsTokenProviderSnippet>

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Text;
using Azure.Core;
using Microsoft.Kiota.Abstractions.Authentication;

namespace SdkSnippets.Snippets;

/// <summary>
/// Implements a custom token provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomTokenProvider"/> class.
/// </remarks>
/// <param name="credential">The token credential to use to request access tokens.</param>
/// <param name="scopes">The permission scopes to use for token requests.</param>
// <CustomTokenProviderSnippet>
public class CustomTokenProvider(
    Azure.Core.TokenCredential credential,
    params string[] scopes) :
    Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider
{
    /// <summary>
    /// Gets an <see cref="AllowedHostsValidator"/> that validates if the
    /// target host of a request is allowed for authentication.
    /// </summary>
    /// <remarks><see cref="AllowedHostsValidator"/> is in the
    /// Microsoft.Kiota.Abstractions.Authentication namespace.
    /// </remarks>
    public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator(
        [
            // Covers all national cloud deployments
            // https://learn.microsoft.com/graph/deployments
            // You can safely remove any endpoints your app doesn't use
            "graph.microsoft.com",
            "graph.microsoft.us",
            "dod-graph.microsoft.us",
            "microsoftgraph.chinacloudapi.cn"
        ]);

    /// <inheritdoc/>
    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        // Check that the request URI is to an allowed host
        if (!AllowedHostsValidator.IsUrlHostValid(uri))
        {
            // Return an empty string
            return string.Empty;
        }

        // Any additional tests - for example
        // require HTTPS
        if (!uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        // If there is a "claims" key in additionalAuthenticationContext
        // decode it and include it in the token request
        // See https://learn.microsoft.com/entra/identity-platform/v2-conditional-access-dev-guide
        string? decodedClaims = null;
        if (additionalAuthenticationContext != null &&
            additionalAuthenticationContext.TryGetValue("claims", out object? claims) &&
            claims is string encodedClaims)
        {
            var decodedBytes = Convert.FromBase64String(encodedClaims);
            decodedClaims = Encoding.UTF8.GetString(decodedBytes);
        }

        var result = await credential.GetTokenAsync(
            new TokenRequestContext(scopes, claims: decodedClaims),
            cancellationToken);

        return result.Token;
    }
}
// </CustomTokenProviderSnippet>

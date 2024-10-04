// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Kiota.Abstractions.Authentication;

namespace SdkSnippets.Snippets;

/// <summary>
/// Implements a custom token provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomAuthenticationProvider"/> class.
/// </remarks>
/// <param name="accessTokenProvider">The token provider to use to acquire access tokens.</param>
// <CustomAuthenticationProviderSnippet>
public class CustomAuthenticationProvider(
    Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider accessTokenProvider) :
    Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider
{
    private IAccessTokenProvider AccessTokenProvider => accessTokenProvider;

    /// <inheritdoc/>
    public async Task AuthenticateRequestAsync(
        Microsoft.Kiota.Abstractions.RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        // The SDK will pass a "claims" key in additionalAuthenticationContext
        // when conditional access policies are applied to a resource.
        // See https://learn.microsoft.com/entra/identity-platform/v2-conditional-access-dev-guide
        // In this case, remove any existing Authorization header
        if (additionalAuthenticationContext != null &&
            additionalAuthenticationContext.ContainsKey("claims") &&
            request.Headers.ContainsKey("Authorization"))
        {
            request.Headers.Remove("Authorization");
        }

        if (!request.Headers.ContainsKey("Authorization"))
        {
            // Get token from your token acquisition method
            // You could:
            // - use the Microsoft.Kiota.Authentication.Azure.AzureIdentityAuthenticationProvider class
            // - implement your own class from Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider
            // - get an access token from some other method
            var token = await AccessTokenProvider.GetAuthorizationTokenAsync(
                request.URI, additionalAuthenticationContext, cancellationToken);

            // If a token is provided, add it in the Authorization header
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }
        }
    }
}
// </CustomAuthenticationProviderSnippet>

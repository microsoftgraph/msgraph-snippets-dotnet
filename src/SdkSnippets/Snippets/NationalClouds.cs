// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Authentication;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides example methods for connecting to national
/// clouds with the Microsoft Graph SDK.
/// </summary>
public static class NationalClouds
{
    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> for the US Government L4 endpoint.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateClientForUsGov()
    {
        // <NationalCloudSnippet>
        // Create the InteractiveBrowserCredential using details
        // from app registered in the Azure AD for US Government portal
        var credential = new InteractiveBrowserCredential(
            "YOUR_TENANT_ID",
            "YOUR_CLIENT_ID",
            new InteractiveBrowserCredentialOptions
            {
                // https://login.microsoftonline.us
                AuthorityHost = AzureAuthorityHosts.AzureGovernment,
                RedirectUri = new Uri("YOUR_REDIRECT_URI"),
            });

        // Create the authentication provider
        var authProvider = new AzureIdentityAuthenticationProvider(
            credential,
            ["https://graph.microsoft.us/.default"]);

        // Create the Microsoft Graph client object using
        // the Microsoft Graph for US Government L4 endpoint
        // NOTE: The API version must be included in the URL
        var graphClient = new GraphServiceClient(
            authProvider,
            "https://graph.microsoft.us/v1.0");
        // </NationalCloudSnippet>

        return graphClient;
    }
}

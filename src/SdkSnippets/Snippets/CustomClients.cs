// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Net;
using Azure.Core;
using Microsoft.Graph;
using Microsoft.Graph.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides methods for creating <see cref="GraphServiceClient"/>
/// instances with custom handlers.
/// </summary>
public static class CustomClients
{
    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with chaos handler.
    /// </summary>
    /// <param name="tokenCredential">The token credential to use to authenticate the client.</param>
    /// <param name="scopes">The Microsoft Graph permission scopes to use for authentication.</param>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithChaosHandler(TokenCredential tokenCredential, string[] scopes)
    {
        // <ChaosHandlerSnippet>
        // tokenCredential is one of the credential classes from Azure.Identity
        // scopes is an array of permission scope strings
        var authProvider = new AzureIdentityAuthenticationProvider(tokenCredential, scopes: scopes);

        var handlers = GraphClientFactory.CreateDefaultHandlers();

        // Remove a default handler
        // Microsoft.Kiota.Http.HttpClientLibrary.Middleware.CompressionHandler
        var compressionHandler =
            handlers.Where(h => h is CompressionHandler).FirstOrDefault();
        handlers.Remove(compressionHandler);

        // Add a new one
        // ChaosHandler simulates random server failures
        // Microsoft.Kiota.Http.HttpClientLibrary.Middleware.ChaosHandler
        handlers.Add(new ChaosHandler());

        var httpClient = GraphClientFactory.Create(handlers);
        var customGraphClient = new GraphServiceClient(httpClient, authProvider);
        // </ChaosHandlerSnippet>

        return customGraphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with an HTTP proxy.
    /// </summary>
    /// <param name="tokenCredential">The token credential to use to authenticate the client.</param>
    /// <param name="scopes">The Microsoft Graph permission scopes to use for authentication.</param>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithProxy(TokenCredential tokenCredential, string[] scopes)
    {
        // <ProxySnippet>
        // URI to proxy
        var proxyAddress = "http://localhost:8888";

        // NOTE: Authentication requests will not go through the proxy.
        // Azure.Identity token credential classes have their own separate method
        // for configuring a proxy using TokenCredentialOptions.Transport
        var authProvider = new AzureIdentityAuthenticationProvider(tokenCredential, scopes);

        // This example works with Microsoft.Graph 5+
        var httpClient = GraphClientFactory.Create(proxy: new WebProxy(new Uri(proxyAddress)));
        var graphClient = new GraphServiceClient(httpClient, authProvider);
        // </ProxySnippet>

        return graphClient;
    }
}

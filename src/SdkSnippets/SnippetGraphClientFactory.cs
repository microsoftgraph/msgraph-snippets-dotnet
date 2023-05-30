// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Authentication;
using Microsoft.Kiota.Abstractions.Authentication;
using SdkClientLogging;
using SdkSnippets.Extensions;

namespace SdkSnippets;

/// <summary>
/// Creates and configures a <see cref="GraphServiceClient"/> instance.
/// </summary>
public static class SnippetGraphClientFactory
{
    private static GraphServiceClient? userClient;

    /// <summary>
    /// Creates and configure a <see cref="GraphServiceClient"/> instance for delegated authentication.
    /// </summary>
    /// <param name="settings">Application settings.</param>
    /// <param name="deviceCodePrompt">Callback function to prompt the user with the device code prompt.</param>
    /// <param name="loggerFactory">Logger factory to be used for debug logging.</param>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static async Task<GraphServiceClient> GetGraphClientForUserAsync(
        Settings settings,
        Func<DeviceCodeInfo, CancellationToken, Task> deviceCodePrompt,
        ILoggerFactory? loggerFactory = null)
    {
        if (userClient == null)
        {
            var options = new DeviceCodeCredentialOptions
            {
                ClientId = settings.ClientId,
                TenantId = settings.TenantId,
                DeviceCodeCallback = deviceCodePrompt,
                TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                {
                    Name = "snippet-token-cache",
                },
            };

            await options.AddAuthenticationRecordIfPresentAsync(settings.AuthCachePath);

            var credential = new DeviceCodeCredential(options);

            if (options.AuthenticationRecord == null)
            {
                await credential.AuthenticateAndCacheRecordAsync(
                    settings.AuthCachePath, settings.GraphUserScopes);
            }

            var authProvider = new AzureIdentityAuthenticationProvider(
                credential, scopes: settings.GraphUserScopes);

            if (settings.DebugLog && loggerFactory != null)
            {
                userClient = CreateGraphClientWithDebugLog(
                    authProvider,
                    loggerFactory,
                    settings.ShowTokens,
                    settings.ShowPayloads);
            }
            else
            {
                userClient = new GraphServiceClient(authProvider);
            }
        }

        return userClient;
    }

    private static GraphServiceClient CreateGraphClientWithDebugLog(
        IAuthenticationProvider authProvider,
        ILoggerFactory loggerFactory,
        bool showTokens,
        bool showPayloads)
    {
        var logger = loggerFactory.CreateLogger<SdkClientDebugLogMiddleware>();

        var handlers = GraphClientFactory.CreateDefaultHandlers();

        handlers.Add(new SdkClientDebugLogMiddleware(logger, showTokens, showPayloads));

        var httpClient = GraphClientFactory.Create(handlers);

        return new GraphServiceClient(httpClient, authProvider);
    }
}

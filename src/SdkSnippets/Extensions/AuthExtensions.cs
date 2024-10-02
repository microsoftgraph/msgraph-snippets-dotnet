// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Core;
using Azure.Identity;

namespace SdkSnippets.Extensions;

/// <summary>
/// Implements extensions to Azure.Identity authentication objects.
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Loads auth cache if present and adds it to the <see cref="DeviceCodeCredentialOptions"/>.
    /// </summary>
    /// <param name="options">The DeviceCodeCredentialOptions to update.</param>
    /// <param name="authCachePath">The path to the auth cache file.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddAuthenticationRecordIfPresentAsync(
        this DeviceCodeCredentialOptions options,
        string? authCachePath)
    {
        if (!string.IsNullOrEmpty(authCachePath))
        {
            try
            {
                // Attempt to load the cached auth record
                using var readCacheStream = new FileStream(authCachePath, FileMode.Open, FileAccess.Read);
                var cachedAuth = await AuthenticationRecord.DeserializeAsync(readCacheStream);
                options.AuthenticationRecord = cachedAuth;
            }
            catch (FileNotFoundException)
            {
                // Silently handle
            }
        }
    }

    /// <summary>
    /// Generates and persists an authentication record.
    /// </summary>
    /// <param name="credential">The <see cref="DeviceCodeCredential"/> to use for authentication.</param>
    /// <param name="authCachePath">The path to the auth cache file.</param>
    /// <param name="graphScopes">The Microsoft Graph permission scopes to use for authentication.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AuthenticateAndCacheRecordAsync(
        this DeviceCodeCredential credential,
        string? authCachePath,
        string[]? graphScopes)
    {
        if (!string.IsNullOrEmpty(authCachePath))
        {
            graphScopes ??= ["https://graph.microsoft.com/.default"];

            var context = new TokenRequestContext(graphScopes);
            var authRecord = await credential.AuthenticateAsync(context);
            if (authRecord != null)
            {
                using var cacheStream = new FileStream(authCachePath, FileMode.Create, FileAccess.Write);
                await authRecord.SerializeAsync(cacheStream);
            }
        }
    }
}

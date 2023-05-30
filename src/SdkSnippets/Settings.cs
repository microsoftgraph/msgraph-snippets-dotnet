// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Configuration;

namespace SdkSnippets;

/// <summary>
/// Represents the app settings loaded from JSON.
/// </summary>
public class Settings
{
    /// <summary>
    /// Gets or sets the client ID for the app registration in Azure.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID for the app registration in Azure.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the Graph permission scopes used for delegated auth.
    /// </summary>
    public string[]? GraphUserScopes { get; set; }

    /// <summary>
    /// Gets or sets the file path to the auth cache.
    /// </summary>
    public string? AuthCachePath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable debug logging.
    /// </summary>
    public bool DebugLog { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show tokens in debug logging.
    /// </summary>
    public bool ShowTokens { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show payloads in debug logging.
    /// </summary>
    public bool ShowPayloads { get; set; }

    /// <summary>
    /// Loads settings from appsettings.json and/or appsettings.Development.json.
    /// </summary>
    /// <returns><see cref="Settings"/>.</returns>
    /// <exception cref="Exception">Indicates that settings could not be loaded.</exception>
    public static Settings LoadSettings()
    {
        // Load settings
        // appsettings.json is required
        // appsettings.Development.json" is optional, values override appsettings.json
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .Build();

        return config.GetRequiredSection("Settings").Get<Settings>() ??
            throw new Exception("Could not load app settings. See README for configuration instructions.");
    }
}

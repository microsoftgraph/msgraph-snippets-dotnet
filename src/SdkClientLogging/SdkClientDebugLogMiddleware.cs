// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using SdkClientLogging.Extensions;

namespace SdkClientLogging;

/// <summary>
/// Microsoft Graph SDK middleware for logging requests and responses.
/// </summary>
public class SdkClientDebugLogMiddleware : DelegatingHandler
{
    private readonly ILogger logger;
    private readonly SdkClientLoggingOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SdkClientDebugLogMiddleware"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    /// <param name="showTokens">Indicates whether to show access tokens in logging output.</param>
    /// <param name="showPayloads">Indicates whether to show payloads in logging output.</param>
    public SdkClientDebugLogMiddleware(
        ILogger logger,
        bool showTokens = false,
        bool showPayloads = false)
    {
        this.logger = logger;
        options = new SdkClientLoggingOptions
        {
            ShowTokens = showTokens,
            ShowPayloads = showPayloads,
        };
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await request.LogToLoggerAsync(logger, options);
        var response = await base.SendAsync(request, cancellationToken);
        await response.LogToLoggerAsync(logger, options);
        return response;
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace SdkClientLogging.Extensions;

/// <summary>
/// Implements logging extensions to <see cref="HttpRequestMessage"/> objects.
/// </summary>
public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Logs HTTP request details.
    /// </summary>
    /// <param name="request">The HTTP request to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <param name="options">Logging options.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task LogToLoggerAsync(
        this HttpRequestMessage request,
        ILogger logger,
        SdkClientLoggingOptions options)
    {
        logger.LogInformation(
            "REQUEST {Method} {AbsoluteUri}",
            request.Method,
            request.RequestUri?.AbsoluteUri);
        request.LogHeadersToLogger(logger, options);

        if (options.ShowPayloads)
        {
            await request.LogPayloadToLoggerAsync(logger);
        }
    }

    /// <summary>
    /// Logs HTTP headers from an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <param name="options">Logging options.</param>
    public static void LogHeadersToLogger(
        this HttpRequestMessage request,
        ILogger logger,
        SdkClientLoggingOptions options)
    {
        foreach (var header in request.Headers.ToList())
        {
            if (!options.ShowTokens &&
                header.Key.Equals("authorization", StringComparison.InvariantCultureIgnoreCase))
            {
                // Do not log authorization header,
                // log placeholder to confirm it's present
                logger.LogInformation("{Key}: Bearer ***", header.Key);
                continue;
            }

            logger.LogInformation("{Key}: {Value}", header.Key, string.Join(',', header.Value));
        }
    }

    /// <summary>
    /// Logs the payload of an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static Task LogPayloadToLoggerAsync(
        this HttpRequestMessage request,
        ILogger logger)
    {
        if (request.Content == null)
        {
            return Task.CompletedTask;
        }

        return request.Content.LogToLoggerAsync(logger);
    }
}

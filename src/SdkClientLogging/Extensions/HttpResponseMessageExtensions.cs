// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace SdkClientLogging.Extensions;

/// <summary>
/// Implements logging extensions to <see cref="HttpResponseMessage"/> objects.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Logs HTTP response details.
    /// </summary>
    /// <param name="response">The HTTP response to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <param name="options">Logging options.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task LogToLoggerAsync(
        this HttpResponseMessage response,
        ILogger logger,
        SdkClientLoggingOptions options)
    {
        logger.LogInformation(
            "RESPONSE {StatusCode} {StatusCodeName}",
            (int)response.StatusCode,
            response.StatusCode);
        response.LogHeadersToLogger(logger, options);

        if (options.ShowPayloads)
        {
            await response.LogPayloadToLoggerAsync(logger, options);
        }
    }

    /// <summary>
    /// Logs HTTP headers from an HTTP response.
    /// </summary>
    /// <param name="response">The HTTP response to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <param name="options">Logging options.</param>
    public static void LogHeadersToLogger(
        this HttpResponseMessage response,
        ILogger logger,
        SdkClientLoggingOptions options)
    {
        foreach (var header in response.Headers.ToList())
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
    /// Logs the payload of an HTTP response.
    /// </summary>
    /// <param name="response">The HTTP response to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <param name="options">Logging options.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task LogPayloadToLoggerAsync(
        this HttpResponseMessage response,
        ILogger logger,
        SdkClientLoggingOptions options)
    {
        if (response.Content != null)
        {
            await response.Content.LogToLoggerAsync(logger, options.ShowTokens);
        }
    }
}

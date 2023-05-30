// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace SdkClientLogging.Extensions;

/// <summary>
/// Implements logging extensions to <see cref="HttpContentExtensions"/> objects.
/// </summary>
public static class HttpContentExtensions
{
    /// <summary>
    /// Extracts content from the HTTP request or response.
    /// </summary>
    /// <param name="content">The <see cref="HttpContent"/> to extract from.</param>
    /// <param name="isGzip">Indicates whether the content is compressed with GZip.</param>
    /// <returns>The string content.</returns>
    public static async Task<string?> ExtractContentAsync(this HttpContent content, bool isGzip)
    {
        if (isGzip)
        {
            // Without this the stream becomes unreadable
            // when it gets to the SDK
            // Thread.Sleep(5000);
            await content.LoadIntoBufferAsync();
            var payloadBytes = await content.ReadAsByteArrayAsync();
            using var memStream = new MemoryStream(payloadBytes);
            using var gzipStream = new GZipStream(memStream, CompressionMode.Decompress);
            var decompressedContent = new StreamContent(gzipStream);
            return await decompressedContent.ReadAsStringAsync();
        }
        else
        {
            return await content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// Logs HTTP payload content.
    /// </summary>
    /// <param name="content">The <see cref="HttpContent"/> to log.</param>
    /// <param name="logger">The logger to use for logging output.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task LogToLoggerAsync(this HttpContent content, ILogger logger)
    {
        bool isGzip = false;

        foreach (var header in content.Headers.ToList())
        {
            logger.LogInformation("{Key}: {Value}", header.Key, string.Join(',', header.Value));

            if (header.Key.Equals("content-encoding", StringComparison.InvariantCultureIgnoreCase) &&
                header.Value.Any(value => value.Equals("gzip", StringComparison.InvariantCultureIgnoreCase)))
            {
                isGzip = true;
            }
        }

        var payload = await content.ExtractContentAsync(isGzip);
        logger.LogInformation("Payload: {Payload}", payload);
    }
}

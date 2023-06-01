// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides example methods for paging with
/// the Microsoft Graph SDK.
/// </summary>
public static class Paging
{
    /// <summary>
    /// Runs all paging samples in this file.
    /// </summary>
    /// <param name="graphClient">An authenticated <see cref="GraphServiceClient"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task RunAllSamples(GraphServiceClient graphClient)
    {
        await IterateAllMessages(graphClient);
        await IterateAllMessagesWithPause(graphClient);
    }

    private static async Task IterateAllMessages(GraphServiceClient graphClient)
    {
        // <PagingSnippet>
        var messages = await graphClient.Me.Messages
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = 10;
                requestConfiguration.QueryParameters.Select =
                    new string[] { "sender", "subject", "body" };
                requestConfiguration.Headers.Add(
                    "Prefer", "outlook.body-content-type=\"text\"");
            });

        if (messages == null)
        {
            return;
        }

        var pageIterator = PageIterator<Message, MessageCollectionResponse>
            .CreatePageIterator(
                graphClient,
                messages,
                // Callback executed for each item in
                // the collection
                (msg) =>
                {
                    Console.WriteLine(msg.Subject);
                    return true;
                },
                // Used to configure subsequent page
                // requests
                (req) =>
                {
                    // Re-add the header to subsequent requests
                    req.Headers.Add("Prefer", "outlook.body-content-type=\"text\"");
                    return req;
                });

        await pageIterator.IterateAsync();
        // </PagingSnippet>
    }

    private static async Task IterateAllMessagesWithPause(GraphServiceClient graphClient)
    {
        // <ResumePagingSnippet>
        int count = 0;
        int pauseAfter = 25;

        var messages = await graphClient.Me.Messages
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = 10;
                requestConfiguration.QueryParameters.Select =
                    new string[] { "sender", "subject" };
            });

        if (messages == null)
        {
            return;
        }

        var pageIterator = PageIterator<Message, MessageCollectionResponse>
            .CreatePageIterator(
                graphClient,
                messages,
                (msg) =>
                {
                    Console.WriteLine(msg.Subject);
                    count++;
                    // If we've iterated over the limit,
                    // stop the iteration by returning false
                    return count < pauseAfter;
                });

        await pageIterator.IterateAsync();

        while (pageIterator.State != PagingState.Complete)
        {
            Console.WriteLine("Iteration paused for 5 seconds...");
            await Task.Delay(5000);
            // Reset count
            count = 0;
            await pageIterator.ResumeAsync();
        }
        // </ResumePagingSnippet>
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides example methods for sending batch requests with
/// the Microsoft Graph SDK.
/// </summary>
public static class BatchRequests
{
    /// <summary>
    /// Runs all batch samples in this file.
    /// </summary>
    /// <param name="graphClient">An authenticated <see cref="GraphServiceClient"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task RunBatchSamples(GraphServiceClient graphClient)
    {
        await SimpleBatch(graphClient);
        await DependentBatch(graphClient);
    }

    /// <summary>
    /// Creates and sends a simple batch request.
    /// </summary>
    /// <param name="graphClient">An authenticated <see cref="GraphServiceClient"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task SimpleBatch(GraphServiceClient graphClient)
    {
        // <SimpleBatchSnippet>
        // Use the request builder to generate a regular
        // request to /me
        var userRequest = graphClient.Me.ToGetRequestInformation();

        var today = DateTime.Now.Date;

        // Use the request builder to generate a regular
        // request to /me/calendarview?startDateTime="start"&endDateTime="end"
        var eventsRequest = graphClient.Me.CalendarView
            .ToGetRequestInformation(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.StartDateTime =
                        today.ToString("yyyy-MM-ddTHH:mm:ssK");
                    requestConfiguration.QueryParameters.EndDateTime =
                        today.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssK");
                });

        // Build the batch
        var batchRequestContent = new BatchRequestContent(graphClient);

        // Using AddBatchRequestStepAsync adds each request as a step
        // with no specified order of execution
        var userRequestId = await batchRequestContent
            .AddBatchRequestStepAsync(userRequest);
        var eventsRequestId = await batchRequestContent
            .AddBatchRequestStepAsync(eventsRequest);

        var returnedResponse = await graphClient.Batch.PostAsync(batchRequestContent);

        // De-serialize response based on known return type
        try
        {
            var user = await returnedResponse
                .GetResponseByIdAsync<User>(userRequestId);
            Console.WriteLine($"Hello {user.DisplayName}!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get user failed: {ex.Message}");
        }

        // For collections, must use the *CollectionResponse class to deserialize
        // The .Value property will contain the *CollectionPage type that the Graph client
        // returns from GetAsync().
        try
        {
            var events = await returnedResponse
                .GetResponseByIdAsync<EventCollectionResponse>(eventsRequestId);
            Console.WriteLine(
                $"You have {events.Value?.Count} events on your calendar today.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get calendar view failed: {ex.Message}");
        }
        // </SimpleBatchSnippet>
    }

    /// <summary>
    /// Creates and sends a dependent batch request.
    /// </summary>
    /// <param name="graphClient">An authenticated <see cref="GraphServiceClient"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task DependentBatch(GraphServiceClient graphClient)
    {
        // <DependentBatchSnippet>
        var today = DateTime.Now.Date;

        var newEvent = new Event
        {
            Subject = "File end-of-day report",
            Start = new DateTimeTimeZone
            {
                // 5:00 PM
                DateTime = today.AddHours(17)
                    .ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = TimeZoneInfo.Local.StandardName,
            },
            End = new DateTimeTimeZone
            {
                // 5:30 PM
                DateTime = today.AddHours(17).AddMinutes(30)
                    .ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = TimeZoneInfo.Local.StandardName,
            },
        };

        // Use the request builder to generate a regular
        // POST request to /me/events
        var addEventRequest = graphClient.Me.Events
            .ToPostRequestInformation(newEvent);

        // Use the request builder to generate a regular
        // request to /me/calendarview?startDateTime="start"&endDateTime="end"
        var calendarViewRequest = graphClient.Me.CalendarView.ToGetRequestInformation(
            requestConfiguration =>
            {
                requestConfiguration.QueryParameters.StartDateTime =
                    today.ToString("yyyy-MM-ddTHH:mm:ssK");
                requestConfiguration.QueryParameters.EndDateTime =
                    today.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssK");
            });

        // Build the batch
        var batchRequestContent = new BatchRequestContent(graphClient);

        // Force the requests to execute in order, so that the request for
        // today's events will include the new event created.

        // First request, no dependency
        var addEventRequestId = await batchRequestContent
            .AddBatchRequestStepAsync(addEventRequest);

        // Second request, depends on addEventRequestId
        var eventsRequestId = Guid.NewGuid().ToString();
        var eventsRequestMessage = await graphClient.RequestAdapter
            .ConvertToNativeRequestAsync<HttpRequestMessage>(calendarViewRequest);
        batchRequestContent.AddBatchRequestStep(new BatchRequestStep(
            eventsRequestId,
            eventsRequestMessage,
            new List<string> { addEventRequestId }));

        var returnedResponse = await graphClient.Batch.PostAsync(batchRequestContent);

        // De-serialize response based on known return type
        try
        {
            var createdEvent = await returnedResponse
                .GetResponseByIdAsync<Event>(addEventRequestId);
            Console.WriteLine($"New event created with ID: {createdEvent.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Add event failed: {ex.Message}");
        }

        // For collections, must use the *CollectionResponse class to deserialize
        // The .Value property will contain the *CollectionPage type that the Graph client
        // returns from GetAsync().
        try
        {
            var events = await returnedResponse
                .GetResponseByIdAsync<EventCollectionResponse>(eventsRequestId);
            Console.WriteLine(
                $"You have {events.Value?.Count} events on your calendar today.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get calendar view failed: {ex.Message}");
        }
        // </DependentBatchSnippet>
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides example methods for creating CRUD requests with
/// the Microsoft Graph SDK.
/// </summary>
public static class CreateRequests
{
    /// <summary>
    /// Runs all request samples in this file.
    /// </summary>
    /// <param name="graphClient">An authenticated <see cref="GraphServiceClient"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task MakeRequests(GraphServiceClient graphClient)
    {
        // Create a new message
        var tempMessage = await graphClient.Me.Messages.PostAsync(
            new Message
            {
                Subject = "Temporary",
            });
        var messageId = tempMessage?.Id ?? throw new Exception("Couldn't create new message");

        // Get a team to update
        var teams = await graphClient.Groups.GetAsync(config =>
        {
            config.QueryParameters.Filter = "resourceProvisioningOptions/Any(x:x+eq+'Team')";
        });
        var teamId = teams?.Value?.FirstOrDefault()?.Id ?? throw new Exception("Couldn't get a team");

        await MakeReadRequest(graphClient);
        await MakeSelectRequest(graphClient);
        await MakeListRequest(graphClient);
        await MakeItemByIdRequest(graphClient, messageId);
        await MakeExpandRequest(graphClient, messageId);
        await MakeDeleteRequest(graphClient, messageId);
        await MakeCreateRequest(graphClient);
        await MakeUpdateRequest(graphClient, teamId);
        await MakeHeadersRequest(graphClient);
        await MakeQueryParametersRequest(graphClient);
    }

    private static async Task<User?> MakeReadRequest(GraphServiceClient graphClient)
    {
        // <ReadRequestSnippet>
        // GET https://graph.microsoft.com/v1.0/me
        var user = await graphClient.Me
            .GetAsync();
        // </ReadRequestSnippet>

        return user;
    }

    private static async Task<User?> MakeSelectRequest(GraphServiceClient graphClient)
    {
        // <SelectRequestSnippet>
        // GET https://graph.microsoft.com/v1.0/me?$select=displayName,jobTitle
        var user = await graphClient.Me
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select =
                    new string[] { "displayName", "jobTitle" };
            });
        // </SelectRequestSnippet>

        return user;
    }

    private static async Task<MessageCollectionResponse?> MakeListRequest(GraphServiceClient graphClient)
    {
        // <ListRequestSnippet>
        // GET https://graph.microsoft.com/v1.0/me/messages?
        // $select=subject,sender&$filter=subject eq 'Hello world'&$orderBy=receivedDateTime
        var messages = await graphClient.Me.Messages
            .GetAsync(requestConfig =>
            {
                requestConfig.QueryParameters.Select =
                    new string[] { "subject", "sender" };
                requestConfig.QueryParameters.Filter =
                    "subject eq 'Hello world'";
            });
        // </ListRequestSnippet>

        return messages;
    }

    private static async Task<Message?> MakeItemByIdRequest(GraphServiceClient graphClient, string messageId)
    {
        // <ItemByIdRequestSnippet>
        // GET https://graph.microsoft.com/v1.0/me/messages/{message-id}
        // messageId is a string containing the id property of the message
        var message = await graphClient.Me.Messages[messageId]
            .GetAsync();
        // </ItemByIdRequestSnippet>

        return message;
    }

    private static async Task<Message?> MakeExpandRequest(GraphServiceClient graphClient, string messageId)
    {
        // <ExpandRequestSnippet>
        // GET https://graph.microsoft.com/v1.0/me/messages/{message-id}?$expand=attachments
        // messageId is a string containing the id property of the message
        var message = await graphClient.Me.Messages[messageId]
            .GetAsync(requestConfig =>
                requestConfig.QueryParameters.Expand = new string[] { "attachments" });
        // </ExpandRequestSnippet>

        return message;
    }

    private static async Task MakeDeleteRequest(GraphServiceClient graphClient, string messageId)
    {
        // <DeleteRequestSnippet>
        // DELETE https://graph.microsoft.com/v1.0/me/messages/{message-id}
        // messageId is a string containing the id property of the message
        await graphClient.Me.Messages[messageId]
            .DeleteAsync();
        // </DeleteRequestSnippet>
    }

    private static async Task<Calendar?> MakeCreateRequest(GraphServiceClient graphClient)
    {
        // <CreateRequestSnippet>
        // POST https://graph.microsoft.com/v1.0/me/calendars
        var calendar = new Calendar
        {
            Name = "Volunteer",
        };

        var newCalendar = await graphClient.Me.Calendars
            .PostAsync(calendar);
        // </CreateRequestSnippet>

        return newCalendar;
    }

    private static async Task MakeUpdateRequest(GraphServiceClient graphClient, string teamId)
    {
        // <UpdateRequestSnippet>
        // PATCH https://graph.microsoft.com/v1.0/teams/{team-id}
        var team = new Team
        {
            FunSettings = new TeamFunSettings
            {
                AllowGiphy = true,
                GiphyContentRating = GiphyRatingType.Strict,
            },
        };

        // teamId is a string containing the id property of the team
        await graphClient.Teams[teamId]
            .PatchAsync(team);
        // </UpdateRequestSnippet>
    }

    private static async Task<EventCollectionResponse?> MakeHeadersRequest(GraphServiceClient graphClient)
    {
        // <HeadersRequestSnippet>
        //  GET https://graph.microsoft.com/v1.0/me/events
        var events = await graphClient.Me.Events
            .GetAsync(requestConfig =>
            {
                requestConfig.Headers.Add(
                    "Prefer", @"outlook.timezone=""Pacific Standard Time""");
                requestConfig.QueryParameters.Select =
                    new string[] { "subject", "body", "bodyPreview" };
            });
        // </HeadersRequestSnippet>

        return events;
    }

    private static async Task<EventCollectionResponse?> MakeQueryParametersRequest(GraphServiceClient graphClient)
    {
        // <QueryParametersRequestSnippet>
        // GET https://graph.microsoft.com/v1.0/me/calendarView?
        // startDateTime=2020-12-01T00:00:00Z&endDateTime=2020-12-30T00:00:00Z
        var events = await graphClient.Me.CalendarView
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.StartDateTime =
                    "2020-12-01T00:00:00Z";
                requestConfiguration.QueryParameters.EndDateTime =
                    "2020-12-30T00:00:00Z";
            });
        // </QueryParametersRequestSnippet>

        return events;
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.ODataErrors;
using SdkSnippets;
using SdkSnippets.Snippets;

var settings = Settings.LoadSettings();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .ClearProviders()
        .AddSimpleConsole(options =>
        {
            options.SingleLine = true;
        });
});

var userClient = await SnippetGraphClientFactory.GetGraphClientForUserAsync(
    settings,
    (info, cancel) =>
    {
        Console.WriteLine(info.Message);
        return Task.CompletedTask;
    },
    loggerFactory);

try
{
    var me = await userClient.Me.GetAsync();
    Console.WriteLine($"Hello, {me?.GivenName}");

    await BatchRequests.RunBatchSamples(userClient);
}
catch (ODataError error)
{
    Console.WriteLine(error.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}

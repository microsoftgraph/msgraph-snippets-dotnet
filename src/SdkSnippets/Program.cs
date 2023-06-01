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
}
catch (ODataError error)
{
    Console.WriteLine(error.Error?.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}

int choice = -1;

while (choice < 0)
{
    Console.WriteLine("Please choose one of the following options:");
    Console.WriteLine("0. Exit");
    Console.WriteLine("1. Run batch samples");
    Console.WriteLine("2. Run create request samples");
    Console.WriteLine("3. Run upload samples");
    Console.WriteLine("4. Run paging samples");

    try
    {
        choice = int.Parse(Console.ReadLine() ?? string.Empty);
    }
    catch (FormatException)
    {
        // Set to invalid value
        choice = -1;
    }

    try
    {
        switch (choice)
        {
            case 0:
                // Exit the program
                Console.WriteLine("Goodbye...");
                break;
            case 1:
                await BatchRequests.RunBatchSamples(userClient);
                break;
            case 2:
                await CreateRequests.MakeRequests(userClient);
                break;
            case 3:
                await LargeFileUpload.RunUploadSamples(userClient);
                break;
            case 4:
                await Paging.RunAllSamples(userClient);
                break;
            default:
                Console.WriteLine("Invalid choice! Please try again.");
                break;
        }
    }
    catch (ODataError error)
    {
        Console.WriteLine(error.Error?.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using AttachmentUpload = Microsoft.Graph.Me.Messages.Item.Attachments.CreateUploadSession;
using DriveUpload = Microsoft.Graph.Drives.Item.Items.Item.CreateUploadSession;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides example methods for large file uploads with
/// the Microsoft Graph SDK.
/// </summary>
public static class LargeFileUpload
{
    /// <summary>
    /// Runs all upload samples in this file.
    /// </summary>
    /// <param name="graphClient">An authenticated <see cref="GraphServiceClient"/>.</param>
    /// <param name="filePath">A path to a large file.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task RunUploadSamples(GraphServiceClient graphClient, string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        var itemPath = "Documents/vacation.gif";

        await UploadFileToOneDrive(graphClient, filePath, itemPath);
        await UploadAttachmentToMessage(graphClient, filePath);
    }

    private static async Task UploadFileToOneDrive(
        GraphServiceClient graphClient,
        string filePath,
        string itemPath)
    {
        // <LargeFileUploadSnippet>
        using var fileStream = File.OpenRead(filePath);

        // Use properties to specify the conflict behavior
        // using DriveUpload = Microsoft.Graph.Drives.Item.Items.Item.CreateUploadSession;
        var uploadSessionRequestBody = new DriveUpload.CreateUploadSessionPostRequestBody
        {
            Item = new DriveItemUploadableProperties
            {
                AdditionalData = new Dictionary<string, object>
                {
                    { "@microsoft.graph.conflictBehavior", "replace" },
                },
            },
        };

        // Create the upload session
        // itemPath does not need to be a path to an existing item
        var myDrive = await graphClient.Me.Drive.GetAsync();
        var uploadSession = await graphClient.Drives[myDrive?.Id]
            .Items["root"]
            .ItemWithPath(itemPath)
            .CreateUploadSession
            .PostAsync(uploadSessionRequestBody);

        // Max slice size must be a multiple of 320 KiB
        int maxSliceSize = 320 * 1024;
        var fileUploadTask = new LargeFileUploadTask<DriveItem>(
            uploadSession, fileStream, maxSliceSize, graphClient.RequestAdapter);

        var totalLength = fileStream.Length;
        // Create a callback that is invoked after each slice is uploaded
        IProgress<long> progress = new Progress<long>(prog =>
        {
            Console.WriteLine($"Uploaded {prog} bytes of {totalLength} bytes");
        });

        try
        {
            // Upload the file
            var uploadResult = await fileUploadTask.UploadAsync(progress);

            Console.WriteLine(uploadResult.UploadSucceeded ?
                $"Upload complete, item ID: {uploadResult.ItemResponse.Id}" :
                "Upload failed");
        }
        catch (ODataError ex)
        {
            Console.WriteLine($"Error uploading: {ex.Error?.Message}");
        }
        // </LargeFileUploadSnippet>

        // Added to remove warning about unused function
        if (myDrive == null)
        {
            await ResumeUpload(fileUploadTask, progress);
        }
    }

    private static async Task ResumeUpload(
        LargeFileUploadTask<DriveItem> fileUploadTask,
        IProgress<long> progress)
    {
        // <ResumeSnippet>
        await fileUploadTask.ResumeAsync(progress);
        // </ResumeSnippet>
    }

    private static async Task UploadAttachmentToMessage(
        GraphServiceClient graphClient,
        string filePath)
    {
        // <UploadAttachmentSnippet>
        // Create message
        var draftMessage = new Message
        {
            Subject = "Large attachment",
        };

        var savedDraft = await graphClient.Me
            .Messages
            .PostAsync(draftMessage);

        using var fileStream = File.OpenRead(filePath);
        var largeAttachment = new AttachmentItem
        {
            AttachmentType = AttachmentType.File,
            Name = Path.GetFileName(filePath),
            Size = fileStream.Length,
        };

        // using AttachmentUpload = Microsoft.Graph.Me.Messages.Item.Attachments.CreateUploadSession;
        var uploadSessionRequestBody = new AttachmentUpload.CreateUploadSessionPostRequestBody
        {
            AttachmentItem = largeAttachment,
        };

        var uploadSession = await graphClient.Me
            .Messages[savedDraft?.Id]
            .Attachments
            .CreateUploadSession
            .PostAsync(uploadSessionRequestBody);

        // Max slice size must be a multiple of 320 KiB
        int maxSliceSize = 320 * 1024;
        var fileUploadTask =
            new LargeFileUploadTask<FileAttachment>(uploadSession, fileStream, maxSliceSize, graphClient.RequestAdapter);

        var totalLength = fileStream.Length;
        // Create a callback that is invoked after each slice is uploaded
        IProgress<long> progress = new Progress<long>(prog =>
        {
            Console.WriteLine($"Uploaded {prog} bytes of {totalLength} bytes");
        });

        try
        {
            // Upload the file
            var uploadResult = await fileUploadTask.UploadAsync(progress);
            Console.WriteLine(uploadResult.UploadSucceeded ? "Upload complete" : "Upload failed");
        }
        catch (ODataError ex)
        {
            Console.WriteLine($"Error uploading: {ex.Error?.Message}");
        }
        // </UploadAttachmentSnippet>
    }
}

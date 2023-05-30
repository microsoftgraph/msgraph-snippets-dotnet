// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace SdkClientLogging;

/// <summary>
/// SDK client logging options.
/// </summary>
public struct SdkClientLoggingOptions
{
    /// <summary>
    /// Indicates whether to show tokens in logging output.
    /// </summary>
    public bool ShowTokens;

    /// <summary>
    /// Indicates whether to show payloads in logging output.
    /// </summary>
    public bool ShowPayloads;
}

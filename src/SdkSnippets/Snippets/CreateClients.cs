// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

namespace SdkSnippets.Snippets;

/// <summary>
/// Provides methods for creating <see cref="GraphServiceClient"/>
/// instances with different authorization providers.
/// </summary>
public static class CreateClients
{
    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with authorization code provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithAuthorizationCode()
    {
        // <AuthorizationCodeSnippet>
        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Values from app registration
        var clientId = "YOUR_CLIENT_ID";
        var clientSecret = "YOUR_CLIENT_SECRET";

        // For authorization code flow, the user signs into the Microsoft
        // identity platform, and the browser is redirected back to your app
        // with an authorization code in the query parameters
        var authorizationCode = "AUTH_CODE_FROM_REDIRECT";

        // using Azure.Identity;
        var options = new AuthorizationCodeCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.authorizationcodecredential
        var authCodeCredential = new AuthorizationCodeCredential(
            tenantId, clientId, clientSecret, authorizationCode, options);

        var graphClient = new GraphServiceClient(authCodeCredential, scopes);
        // </AuthorizationCodeSnippet>

        return graphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with client secret provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithClientSecret()
    {
        // <ClientSecretSnippet>
        // The client credentials flow requires that you request the
        // /.default scope, and pre-configure your permissions on the
        // app registration in Azure. An administrator must grant consent
        // to those permissions beforehand.
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Values from app registration
        var clientId = "YOUR_CLIENT_ID";
        var clientSecret = "YOUR_CLIENT_SECRET";

        // using Azure.Identity;
        var options = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);

        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
        // </ClientSecretSnippet>

        return graphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with client certificate provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithClientCertificate()
    {
        // <ClientCertificateSnippet>
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Values from app registration
        var clientId = "YOUR_CLIENT_ID";
        var clientCertificate = new X509Certificate2("MyCertificate.pfx");

        // using Azure.Identity;
        var options = new ClientCertificateCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.clientcertificatecredential
        var clientCertCredential = new ClientCertificateCredential(
            tenantId, clientId, clientCertificate, options);

        var graphClient = new GraphServiceClient(clientCertCredential, scopes);
        // </ClientCertificateSnippet>

        return graphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with on-behalf-of provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithOnBehalfOf()
    {
        // <OnBehalfOfSnippet>
        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Values from app registration
        var clientId = "YOUR_CLIENT_ID";
        var clientSecret = "YOUR_CLIENT_SECRET";

        // using Azure.Identity;
        var options = new OnBehalfOfCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        // This is the incoming token to exchange using on-behalf-of flow
        var oboToken = "JWT_TOKEN_TO_EXCHANGE";

        var onBehalfOfCredential = new OnBehalfOfCredential(
            tenantId, clientId, clientSecret, oboToken, options);

        var graphClient = new GraphServiceClient(onBehalfOfCredential, scopes);
        // </OnBehalfOfSnippet>

        return graphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with device code provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithDeviceCode()
    {
        // <DeviceCodeSnippet>
        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Value from app registration
        var clientId = "YOUR_CLIENT_ID";

        // using Azure.Identity;
        var options = new DeviceCodeCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            ClientId = clientId,
            TenantId = tenantId,
            // Callback function that receives the user prompt
            // Prompt contains the generated device code that user must
            // enter during the auth process in the browser
            DeviceCodeCallback = (code, cancellation) =>
            {
                Console.WriteLine(code.Message);
                return Task.FromResult(0);
            },
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
        var deviceCodeCredential = new DeviceCodeCredential(options);

        var graphClient = new GraphServiceClient(deviceCodeCredential, scopes);
        // </DeviceCodeSnippet>

        return graphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with integrated Windows provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithIntegratedWindows()
    {
        // <IntegratedWindowsSnippet>
        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Value from app registration
        var clientId = "YOUR_CLIENT_ID";

        var authenticationProvider = new BaseBearerTokenAuthenticationProvider(
            new IntegratedWindowsTokenProvider(clientId, tenantId));

        var graphClient = new GraphServiceClient(authenticationProvider);

        return graphClient;
        // </IntegratedWindowsSnippet>
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with interactive provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithInteractive()
    {
        // <InteractiveSnippet>
        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Value from app registration
        var clientId = "YOUR_CLIENT_ID";

        // using Azure.Identity;
        var options = new InteractiveBrowserCredentialOptions
        {
            TenantId = tenantId,
            ClientId = clientId,
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            // MUST be http://localhost or http://localhost:PORT
            // See https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/System-Browser-on-.Net-Core
            RedirectUri = new Uri("http://localhost"),
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.interactivebrowsercredential
        var interactiveCredential = new InteractiveBrowserCredential(options);

        var graphClient = new GraphServiceClient(interactiveCredential, scopes);
        // </InteractiveSnippet>

        return graphClient;
    }

    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> with username/password provider.
    /// </summary>
    /// <returns><see cref="GraphServiceClient"/>.</returns>
    public static GraphServiceClient CreateWithUserNamePassword()
    {
        // <UserNamePasswordSnippet>
        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "common";

        // Value from app registration
        var clientId = "YOUR_CLIENT_ID";

        // using Azure.Identity;
        var options = new UsernamePasswordCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        var userName = "adelev@contoso.com";
        var password = "Password1!";

        // https://learn.microsoft.com/dotnet/api/azure.identity.usernamepasswordcredential
        var userNamePasswordCredential = new UsernamePasswordCredential(
            userName, password, tenantId, clientId, options);

        var graphClient = new GraphServiceClient(userNamePasswordCredential, scopes);
        // </UserNamePasswordSnippet>

        return graphClient;
    }
}

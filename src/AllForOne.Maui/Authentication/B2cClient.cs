using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;

namespace AllForOne.Authentication;

/// <summary>
/// This is a wrapper for PublicClientApplication. It is singleton.
/// </summary>    
public class B2cClient
{
    /// <summary>
    /// This is the singleton used by ux. Since PCAWrapper constructor does not have perf or memory issue, it is instantiated directly.
    /// </summary>
    public static B2cClient Instance { get; private set; } = new B2cClient();

    /// <summary>
    /// Instance of PublicClientApplication. It is provided, if App wants more customization.
    /// </summary>
    internal IPublicClientApplication client { get; }

    // private constructor for singleton
    private B2cClient()
    {
        // Create PCA once. Make sure that all the config parameters below are passed
        client = PublicClientApplicationBuilder
                                    .Create(B2CConstants.ClientID)
                                    .WithExperimentalFeatures() // this is for upcoming logger
                                    .WithLogging(_logger)
                                    .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
                                    .WithIosKeychainSecurityGroup(B2CConstants.IOSKeyChainGroup)
                                    .WithRedirectUri(B2CConstants.RedirectUri)
                                    .Build();
    }

    /// <summary>
    /// Acquire the token silently
    /// </summary>
    /// <param name="scopes">desired scopes</param>
    /// <returns>Authentication result</returns>
    public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes)
    {
        // Get accounts by policy
        IEnumerable<IAccount> accounts = await client.GetAccountsAsync(B2CConstants.PolicySignUpSignIn).ConfigureAwait(false);

        // If the token is expired and the refresh token is valid both will be renewed silently
        AuthenticationResult authResult = await client.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
           .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
           .ExecuteAsync()
           .ConfigureAwait(false);

        return authResult;
    }

    /// <summary>
    /// Perform the interactive acquisition of the token for the given scope
    /// </summary>
    /// <param name="scopes">desired scopes</param>
    /// <returns></returns>
    internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
    {
        return await client.AcquireTokenInteractive(B2CConstants.Scopes)
                                                    .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                                                    .ExecuteAsync()
                                                    .ConfigureAwait(false);
    }

    /// <summary>
    /// It will sign out the user.
    /// </summary>
    /// <returns></returns>
    internal async Task SignOutAsync()
    {
        var accounts = await client.GetAccountsAsync().ConfigureAwait(false);
        foreach (var acct in accounts)
        {
            await client.RemoveAsync(acct).ConfigureAwait(false);
        }
    }

    // Todo: Custom logger for sample
    private MyLogger _logger = new MyLogger();

    // Custom logger class
    private class MyLogger : IIdentityLogger
    {
        /// <summary>
        /// Checks if log is enabled or not based on the Entry level
        /// </summary>
        /// <param name="eventLogLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(EventLogLevel eventLogLevel)
        {
            //Try to pull the log level from an environment variable
            var msalEnvLogLevel = Environment.GetEnvironmentVariable("MSAL_LOG_LEVEL");

            EventLogLevel envLogLevel = EventLogLevel.Informational;
            Enum.TryParse<EventLogLevel>(msalEnvLogLevel, out envLogLevel);

            return envLogLevel <= eventLogLevel;
        }

        /// <summary>
        /// Log to console for demo purpose
        /// </summary>
        /// <param name="entry">Log Entry values</param>
        public void Log(LogEntry entry)
        {
            Console.WriteLine(entry.Message);
        }
    }

}

// Todo: Move this to config
public static class B2CConstants
{
    // Azure AD B2C Coordinates
    public const string Tenant = "testorg1234fds3.onmicrosoft.com";
    public const string AzureADB2CHostname = "testorg1234fds3.b2clogin.com";
    public const string ClientID = "cd01c71b-2101-41fa-9528-1695420fbbe1";
    public static readonly string RedirectUri = $"msal{ClientID}://auth";
    public const string PolicySignUpSignIn = "b2c_1_susi";

    public static readonly string[] Scopes = { "https://testorg1234fds3.onmicrosoft.com/1352120a-4fbb-4fee-bb40-b6e55edd4629/user_impersonation" };

    public static readonly string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
    public static readonly string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";

    public const string IOSKeyChainGroup = "com.microsoft.adalcache";
}
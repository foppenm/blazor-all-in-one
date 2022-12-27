using AllForOne.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace AllForOne.Maui.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly B2cSettings b2cSettings;
        private readonly PlatformConfig platformConfig;

        private IPublicClientApplication client { get; }

        public AuthenticationService(IConfiguration configuration, PlatformConfig platformConfig)
        {
            b2cSettings = configuration.GetRequiredSection(B2cSettings.SectionName).Get<B2cSettings>();

            // Create PCA once. Make sure that all the config parameters below are passed
            client = PublicClientApplicationBuilder
                                        .Create(b2cSettings.ClientID)
                                        .WithExperimentalFeatures() // this is for upcoming logger
                                        .WithB2CAuthority(b2cSettings.AuthoritySignInSignUp)
                                        .WithIosKeychainSecurityGroup(b2cSettings.IOSKeyChainGroup)
                                        .WithRedirectUri(b2cSettings.RedirectUri)
                                        .Build();

            this.platformConfig = platformConfig;
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes)
        {
            // Get accounts by policy
            IEnumerable<IAccount> accounts = await client.GetAccountsAsync(b2cSettings.PolicySignUpSignIn).ConfigureAwait(false);

            // If the token is expired and the refresh token is valid both will be renewed silently
            AuthenticationResult authResult = await client.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
               .WithB2CAuthority(b2cSettings.AuthoritySignInSignUp)
               .ExecuteAsync()
               .ConfigureAwait(false);

            return authResult;
        }

        /// <summary>
        /// Perform the interactive acquisition of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
        {
            // Todo: Catch exceptions like "User canceled authentication"
            return await client.AcquireTokenInteractive(b2cSettings.Scopes)
                                                        .WithParentActivityOrWindow(platformConfig.ParentWindow)
                                                        .ExecuteAsync()
                                                        .ConfigureAwait(false);
        }

        /// <summary>
        /// It will sign out the user.
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            var accounts = await client.GetAccountsAsync().ConfigureAwait(false);
            foreach (var acct in accounts)
            {
                await client.RemoveAsync(acct).ConfigureAwait(false);
            }
        }
    }
}

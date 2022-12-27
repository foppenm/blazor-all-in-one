using System.Runtime;

namespace AllForOne.Maui.Authentication
{
    public class B2cSettings
    {
        public const string SectionName = "AzureAd";
        public string Tenant { get; set; } = string.Empty;
        public string AzureADB2CHostname { get; set; }
        public string ClientID { get; set; }
        public string RedirectUri
        {
            get
            {
                return $"msal{ClientID}://auth";
            }
        }
        public string PolicySignUpSignIn { get; set; }
        public string[] Scopes { get; set; }
        public string AuthorityBase
        {
            get
            {
                return $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
            }
        }
        
        public string AuthoritySignInSignUp
        {
            get
            {
                return $"{AuthorityBase}{PolicySignUpSignIn}";
            }
        }

        public string IOSKeyChainGroup { get; set; }
    }

}

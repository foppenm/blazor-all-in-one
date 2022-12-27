using Microsoft.Identity.Client;

namespace AllForOne.Maui.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes);
        Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes);
        Task SignOutAsync();
    }
}
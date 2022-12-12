using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading.Tasks;
using AllForOne.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;

namespace AllForOne.Maui.Authentication;

public class B2cAuthenticationStateProvider : AuthenticationStateProvider
{
    //https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/security/?view=aspnetcore-6.0&pivots=maui#handle-authentication-within-the-blazorwebview-option-2

    private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(currentUser));

    public Task LogInAsync()
    {
        var loginTask = LogInAsyncCore();
        NotifyAuthenticationStateChanged(loginTask);

        return loginTask;

        async Task<AuthenticationState> LogInAsyncCore()
        {
            var user = await LoginWithExternalProviderAsync();
            currentUser = user;

            var state = new AuthenticationState(currentUser);
            return state;
        }
    }

    private async Task<ClaimsPrincipal> LoginWithExternalProviderAsync()
    {
        /*
            Provide OpenID/MSAL code to authenticate the user. See your identity 
            provider's documentation for details.

            Return a new ClaimsPrincipal based on a new ClaimsIdentity.
        */
        AuthenticationResult result = null;

        try
        {
            // First attempt silent login, which checks the cache for an existing valid token.
            // If this is very first time or user has signed out, it will throw MsalUiRequiredException
            result = await B2cClient.Instance.AcquireTokenSilentAsync(B2CConstants.Scopes).ConfigureAwait(false);
        }
        catch (MsalUiRequiredException)
        {
            // This executes UI interaction to obtain token
            result = await B2cClient.Instance.AcquireTokenInteractiveAsync(B2CConstants.Scopes).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Todo: Log
            Console.WriteLine($"Caught exception... - {ex.ToString()}");
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        if (result == null)
            throw new Exception("authentication result is null");

        // Re-map the name claim so that blazor authentication context can pick it up to display
        //http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name -> name
        var editableList = result.ClaimsPrincipal.Claims.ToList();
        editableList.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", editableList.First(x => x.Type == "name").Value));

        // hack: https://stackoverflow.com/questions/20254796/why-is-my-claimsidentity-isauthenticated-always-false-for-web-api-authorize-fil
        var cp = new ClaimsPrincipal(new ClaimsIdentity(editableList, "Bearer"));
        return cp;
    }

    public async Task Logout()
    {
        _ = await B2cClient.Instance.SignOutAsync().ContinueWith(async (t) =>
        {

        }).ConfigureAwait(false);


        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));
    }
}

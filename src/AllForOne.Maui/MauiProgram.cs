using AllForOne.Maui.Authentication;
using AllForOne.Maui.Data;
using AllForOne.Models.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace AllForOne.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        // Set these redirect urls in your app reg in b2c
        // http://localhost is used by WPF applications.
        // https://login.microsoftonline.com/common/oauth2/nativeclient is used by UWP applications.
        // msal{client-id}://auth is used by mobile (Android and iOS) applications.

        // https://github.com/Azure-Samples/active-directory-xamarin-native-v2

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		    builder.Logging.AddDebug();
#endif

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, B2cAuthenticationStateProvider>();

        // Link the interface from the razor to an actual implementation for wasm
        builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

        return builder.Build();
    }
}
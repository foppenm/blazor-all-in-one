using AllForOne.BlazorWasm;
using AllForOne.BlazorWasm.Data;
using AllForOne.Models.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri("https://localhost:5003"))
   .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

    // Ask for the scope to access the api
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://testorg1234fds3.onmicrosoft.com/1352120a-4fbb-4fee-bb40-b6e55edd4629/user_impersonation");
    
    // This removes the popup and redirects to b2c login. This way  add blockers wont block the login
    options.ProviderOptions.LoginMode = "redirect";
});

// Link the interface from the razor to an actual implementation for wasm
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

await builder.Build().RunAsync();
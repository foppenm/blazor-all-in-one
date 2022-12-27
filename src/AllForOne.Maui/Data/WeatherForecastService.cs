using AllForOne.Maui.Authentication;
using AllForOne.Models.Data;
using AllForOne.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net.Http.Json;

namespace AllForOne.Maui.Data;

public class WeatherForecastService: IWeatherForecastService
{
    private readonly B2cSettings b2cSettings;
    private readonly string apiBaseUrl;
    private readonly IAuthenticationService authenticationService;

    public WeatherForecastService(
        IConfiguration configuration, 
        IAuthenticationService authenticationService)
    {
        this.b2cSettings = configuration.GetRequiredSection(B2cSettings.SectionName).Get<B2cSettings>();
        this.apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        this.authenticationService = authenticationService;
    }

    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        AuthenticationResult result = await authenticationService.AcquireTokenSilentAsync(b2cSettings.Scopes).ConfigureAwait(false);
        
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);
            return await client.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
        }
    }
}
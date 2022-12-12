using AllForOne.Authentication;
using AllForOne.Models.Data;
using AllForOne.Models.Interfaces;
using Microsoft.Identity.Client;
using System.Net.Http.Json;

namespace AllForOne.Maui.Data;

public class WeatherForecastService: IWeatherForecastService
{
    public WeatherForecastService() { }

    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        AuthenticationResult result = await B2cClient.Instance.AcquireTokenSilentAsync(B2CConstants.Scopes).ConfigureAwait(false);
        
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:5003/");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);
            return await client.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
        }
    }
}
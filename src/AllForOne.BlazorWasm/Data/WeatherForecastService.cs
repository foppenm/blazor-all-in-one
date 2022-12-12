using AllForOne.Models.Data;
using AllForOne.Models.Interfaces;
using System.Net.Http.Json;

namespace AllForOne.BlazorWasm.Data;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient httpClient;

    public WeatherForecastService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public Task<WeatherForecast[]> GetForecastAsync()
    {
        return httpClient.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
    }
}

using AllForOne.Models.Data;

namespace AllForOne.Models.Interfaces;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> GetForecastAsync();
}

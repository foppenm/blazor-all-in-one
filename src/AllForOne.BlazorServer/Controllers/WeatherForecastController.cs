using AllForOne.Models.Data;
using AllForOne.Models.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllForOne.BlazorServer.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase, IWeatherForecastService
{
    private readonly IWeatherForecastService weatherForecastService;

    public WeatherForecastController(IWeatherForecastService weatherForecastService) => this.weatherForecastService = weatherForecastService;

    [HttpGet]
    public Task<WeatherForecast[]> GetForecastAsync() => weatherForecastService.GetForecastAsync();
}

using Donatas.Core.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Obsolete("this version will not be supported soon")]
    public class WeatherForecastControllerV1(ILogger<WeatherForecastControllerV1> logger) : CoreController
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

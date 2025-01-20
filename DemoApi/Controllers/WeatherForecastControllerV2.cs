using Donatas.Core.Authorization;
using Donatas.Core.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [ApiVersion("2")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : CoreController
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet]
        [Obsolete("This is going to be removed soon", false)]
        [Authorize(Roles = Roles.Reader)] // Direct role assignment without inheritance
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

        [HttpGet]
        [Authorize(Policy = Roles.Contributor)] // Default role policy with inheritance - Owners and Contributors can access this method
        public IEnumerable<WeatherForecast> Get2()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Authorize(Policy = Roles.Reader)]
        public ActionResult<OutputEnum> UsageOfEnums(InputEnum inputEnum)
        {
            var val = (int)inputEnum;
            return Ok((OutputEnum)val);
        }

        [HttpGet]
        [Authorize(Policy = Roles.Reader)]
        public IActionResult TryGetUnhandledException()
        {
            var a = 1;
            var b = 0;
            return Ok(a / b);
        }

        public enum InputEnum
        {
            Property1,
            Property2,
            Property3
        }

        public enum OutputEnum
        {
            Return1,
            Return2,
            Return3,
            Return4
        }
    }
}

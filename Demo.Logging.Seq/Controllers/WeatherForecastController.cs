using Demo.Logging.Seq.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Logging.Seq.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IEmailService _emailService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;

            _logger.LogInformation($"{nameof(WeatherForecastController)} has been instantiated.");
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation($"{nameof(Get)} has been invoked.");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("Throw")]
        public IActionResult Throw() => throw new Exception("Sample exception.");

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error-development")]
        public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
        {
            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            _emailService.SendEmail("admin@email.com", $"Alert: {exceptionHandlerFeature.Error.Message}", exceptionHandlerFeature.Error.StackTrace ?? exceptionHandlerFeature.Error.Message);

            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        }

        /// <summary>
        /// The HandleError action sends an RFC 7807-compliant payload to the client.
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError() => Problem();
    }
}
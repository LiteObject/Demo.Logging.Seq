using Demo.Logging.Seq.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Logging.Seq.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IEmailService _emailService;

        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailService emailService, IWeatherForecastService weatherForecastService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));

            _logger.LogInformation($"{nameof(WeatherForecastController)} has been instantiated.");
        }

        [HttpGet("", Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            _logger.LogInformation($"{nameof(Get)} has been invoked.");

            var weatherForecast = _weatherForecastService.GetByCity(string.Empty);

            return Ok(weatherForecast);
        }

        [HttpGet("{city}", Name = "GetWeatherForecastByCity")]
        public IActionResult GetByCity([FromRoute] string city)
        {
            _logger.LogInformation($"{nameof(GetByCity)} has been invoked.");

            var weatherForecast = _weatherForecastService.GetByCity(city);

            return Ok(weatherForecast);
        }

        [HttpPost]
        public IActionResult Post(WeatherForecast weatherForecast)
        {
            _logger.LogInformation($"{nameof(Post)} has been invoked.");
            return CreatedAtAction(nameof(GetByCity), new { city = weatherForecast.City }, weatherForecast);
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
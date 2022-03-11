namespace Demo.Logging.Seq.Services
{
    public interface IWeatherForecastService
    {
        public IEnumerable<WeatherForecast> GetByCity(string city);
    }
}

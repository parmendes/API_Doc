namespace MyBackstageAPI;

public class WeatherForecast
{
    /// <summary>
    /// The date of the forecast.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Temperature in Celsius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// A short weather description.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// A short weather description.
    /// </summary>
    public string? Summary { get; set; }
}

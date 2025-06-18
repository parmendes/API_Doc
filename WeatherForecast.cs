using System.ComponentModel.DataAnnotations;

namespace MyBackstageAPI;

public class WeatherForecast
{
    /// <summary>
    /// The date of the forecast.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Temperature in Celsius.
    /// </summary>
    [Range(-100, 100)]
    public int TemperatureC { get; set; }

    /// <summary>
    /// The temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// A short weather description.
    /// </summary>
    public string? Summary { get; set; }
}

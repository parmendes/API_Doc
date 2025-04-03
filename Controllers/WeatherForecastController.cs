using Microsoft.AspNetCore.Mvc;

namespace MyBackstageAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the weather forecast (available in versions 1 and 2).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    /// <summary>
    /// Exclusive method for version 2: returns the list of weather summaries.
    /// </summary>
    [HttpGet("summaries")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSummaryList()
    {
        return Ok(Summaries);
    }

    /// <summary>
    /// Adds a new weather forecast.
    /// </summary>
    /// <param name="forecast">The weather forecast to add.</param>
    /// <response code="201">Returns the newly created weather forecast.</response>
    /// <remarks>
    /// Sample request:
    /// 
    /// ```json
    /// POST /WeatherForecast
    /// {
    ///     "Date": "2022-01-01T00:00:00",
    ///     "TemperatureC": 23,
    ///     "TemperatureF": 74,
    ///     "Summary": "Sunny"
    /// }
    /// ``` 
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Post([FromBody] WeatherForecast forecast)
    {
        return CreatedAtAction(nameof(Get), new { id = forecast.Date }, forecast);
    }

    /// <summary>
    /// Updates an existing weather forecast.
    /// </summary>
    [HttpPut("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Put(DateTime date, [FromBody] WeatherForecast forecast)
    {
        var existingForecast = FindForecastByDate(date);
        if (existingForecast == null)
        {
            return NotFound();
        }

        existingForecast.TemperatureC = forecast.TemperatureC;
        existingForecast.Summary = forecast.Summary;

        return Ok(existingForecast);
    }

    /// <summary>
    /// Removes a weather forecast.
    /// </summary>
    [HttpDelete("{date}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(DateTime date)
    {
        return NoContent();
    }

    private static WeatherForecast? FindForecastByDate(DateTime date) => new()
    {
        Date = date,
        TemperatureC = 20,
        Summary = "Sunny"
    };
}

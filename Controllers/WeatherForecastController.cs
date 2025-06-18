    using Microsoft.AspNetCore.Mvc;

namespace MyBackstageAPI.Controllers;

// This attribute marks this class as an API controller, meaning it will handle HTTP requests.
// The ApiVersion attributes specify that this controller supports API versions 1.0 and 2.0.
// The Route attribute defines the base URL for this controller, including the version placeholder.
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WeatherForecastController : ControllerBase
{
    // This is a static array of weather summaries that will be used to generate random weather data.
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    // This is a logger that will be used to log information about the controller's operations.
    private readonly ILogger<WeatherForecastController> _logger;

    // Constructor for the controller. The logger is injected here using dependency injection.
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests to retrieve a list of weather forecasts.
    /// This method is available in both API versions 1.0 and 2.0.
    /// </summary>
    /// <returns>A list of weather forecasts.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)] // Indicates that this method returns a 200 OK status.
    public IEnumerable<WeatherForecast> Get()
    {
        // Generates a list of 5 random weather forecasts.
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index), // Sets the date to today plus the index.
            TemperatureC = Random.Shared.Next(-20, 55), // Generates a random temperature in Celsius.
            Summary = Summaries[Random.Shared.Next(Summaries.Length)] // Picks a random summary from the array.
        })
        .ToArray(); // Converts the result to an array.
    }

    /// <summary>
    /// Handles GET requests to retrieve the list of weather summaries.
    /// This method is only available in API version 2.0.
    /// </summary>
    /// <returns>A list of weather summaries.</returns>
    [HttpGet("summaries")]
    [MapToApiVersion("2.0")] // Specifies that this method is only available in version 2.0 of the API.
    [ProducesResponseType(StatusCodes.Status200OK)] // Indicates that this method returns a 200 OK status.
    public IActionResult GetSummaryList()
    {
        // Returns the list of weather summaries as a response.
        return Ok(Summaries);
    }

    /// <summary>
    /// Handles POST requests to add a new weather forecast.
    /// </summary>
    /// <param name="forecast">The weather forecast to add.</param>
    /// <returns>The newly created weather forecast.</returns>
    /// <remarks>
    /// Example of a request body:
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
    [ProducesResponseType(StatusCodes.Status201Created)] // Indicates that this method returns a 201 Created status.
    public IActionResult Post([FromBody] WeatherForecast forecast)
    {
        // Validates the incoming forecast data.
        // If the model state is invalid, it returns a BadRequest response with the validation errors
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Returns a response indicating that the resource was created successfully.
        return CreatedAtAction(nameof(Get), new { id = forecast.Date }, forecast);
    }

    /// <summary>
    /// Handles PUT requests to update an existing weather forecast.
    /// </summary>
    /// <param name="date">The date of the forecast to update.</param>
    /// <param name="forecast">The updated weather forecast data.</param>
    /// <returns>The updated weather forecast or a 404 Not Found status if the forecast doesn't exist.</returns>
    [HttpPut("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Indicates that this method returns a 200 OK status.
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Indicates that this method returns a 404 Not Found status if the forecast doesn't exist.
    public IActionResult Put(DateTime date, [FromBody] WeatherForecast forecast)
    {
        // Tries to find an existing forecast by the given date.
        var existingForecast = FindForecastByDate(date);
        if (existingForecast == null)
        {
            // Returns a 404 Not Found status if the forecast doesn't exist.
            return NotFound();
        }

        // Updates the existing forecast with the new data.
        existingForecast.TemperatureC = forecast.TemperatureC;
        existingForecast.Summary = forecast.Summary;

        // Returns the updated forecast.
        return Ok(existingForecast);
    }

    /// <summary>
    /// Handles DELETE requests to remove a weather forecast.
    /// </summary>
    /// <param name="date">The date of the forecast to delete.</param>
    /// <returns>A 204 No Content status.</returns>
    [HttpDelete("{date}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Indicates that this method returns a 204 No Content status.
    public IActionResult Delete(DateTime date)
    {
        // Returns a 204 No Content status to indicate that the resource was deleted successfully.
        return NoContent();
    }

    // This is a helper method to simulate finding a weather forecast by date.
    // In a real application, this would query a database or another data source.
    private static WeatherForecast? FindForecastByDate(DateTime date) => new()
    {
        Date = date,
        TemperatureC = 20,
        Summary = "Sunny"
    };
}
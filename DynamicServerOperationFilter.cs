using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Operation filter to dynamically set the server URL based on the current HTTP request.
/// This is useful for generating Swagger documentation with the correct server URL,
/// </summary>
public class DynamicServerOperationFilter : IOperationFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DynamicServerOperationFilter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var serverUrl = $"{request.Scheme}://{request.Host.Value}";
            operation.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
        }
    }
}
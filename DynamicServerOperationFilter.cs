using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            var baseUrl = $"{request.Scheme}://{request.Host}";

            operation.Servers =
            [
                new OpenApiServer { Url = baseUrl, Description = "Dynamic server URL" }
            ];
        }
    }
}
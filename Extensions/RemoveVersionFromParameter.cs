using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Operation filter to remove the "version" parameter from API operations.
/// This is useful when you want to simplify the API documentation by not showing the version parameter,
/// </summary>
public class RemoveVersionFromParameter : IOperationFilter
{
    /// <summary>
    /// Applies the operation filter to remove the "version" parameter from the OpenAPI operation.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to modify.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var versionParameter = operation.Parameters?.FirstOrDefault(p => p.Name == "version");
        if (versionParameter != null)
        {
            operation.Parameters.Remove(versionParameter);
        }
    }
}
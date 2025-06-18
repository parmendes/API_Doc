using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Operation filter to replace the version placeholder in the path with the exact version value.
/// This is useful for generating Swagger documentation with the actual version number in the path,
/// </summary>
public class ReplaceVersionWithExactValueInPath : IDocumentFilter
{
    /// <summary>
    /// Applies the document filter to replace the version placeholder in the OpenAPI document paths with the actual version value.
    /// This is useful for generating Swagger documentation with the actual version number in the path,
    /// </summary>
    /// <param name="swaggerDoc"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();
        foreach (var (key, value) in swaggerDoc.Paths)
        {
            paths.Add(key.Replace("v{version}", $"v{swaggerDoc.Info.Version}"), value);
        }
        swaggerDoc.Paths = paths;
    }
}
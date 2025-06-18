namespace MyBackstageAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a global exception handler that returns a generic error response.
    /// </summary>
    /// <param name="app"> The application builder to configure.</param>
    /// <returns> The configured application builder.</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
            });
        });
        return app;
    }
}
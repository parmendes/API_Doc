using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using MyBackstageAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Service registrations
builder.Services.AddControllers(); // Adds support for controllers, which handle HTTP requests.
builder.Services.AddAuthorization(); // Adds support for securing endpoints with authorization.
builder.Services.AddCustomCors(builder.Configuration); // Adds a custom CORS policy to allow requests from any origin, header, and method.
// builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiVersioningWithExplorer(); // Adds API versioning support and configures it to report versions and assume a default version when unspecified.
builder.Services.AddEndpointsApiExplorer(); // Adds support for exploring endpoints, which is useful for generating API documentation.
builder.Services.AddHttpContextAccessor(); // Adds an HTTP context accessor, which allows access to the current HTTP context in services and middleware.
builder.Services.AddSwaggerWithXmlCommentsAndVersioning(); // Adds Swagger support with XML comments and versioning for API documentation.

var app = builder.Build();

// Middleware pipeline
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS for secure communication.
app.UseCors("AllowAllOrigins"); // Applies the CORS policy defined earlier, allowing requests from any origin, header, and method.
app.UseAuthentication(); // Enables authentication middleware to validate user credentials and issue tokens.
app.UseAuthorization(); // Enables authorization middleware to secure endpoints.
// app.UseRouting(); // Enables routing to match incoming requests to endpoints.


// Configuring Swagger
// Swagger is only enabled in development to avoid exposing documentation in production.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger(); // Enables the Swagger middleware to serve the generated documentation.

    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"Minimal REST API {description.ApiVersion}");
        }
    });
}

app.MapControllers(); // Map controller endpoints to the routing system.
app.Run(); 
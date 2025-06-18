using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Setting up CORS (Cross-Origin Resource Sharing)
// CORS allows the API to accept requests from other domains. This is useful for enabling communication
// between the API and frontend applications hosted on different servers or domains.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Accept requests from any domain.
              .AllowAnyHeader() // Accept any HTTP headers.
              .AllowAnyMethod(); // Accept any HTTP methods (GET, POST, PUT, DELETE, etc.).
    });
});

// Adding essential services to the application
// These services are required for the API to handle HTTP requests and implement authorization.
builder.Services.AddControllers(); // Adds support for controllers, which handle HTTP requests.
builder.Services.AddAuthorization(); // Adds support for securing endpoints with authorization.

// Configuring API versioning
// This setup allows the API to support multiple versions, making it easier to manage changes over time.
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true; // Adds version information to the response headers.
    options.AssumeDefaultVersionWhenUnspecified = true; // Uses a default version if the client doesn't specify one.
    options.DefaultApiVersion = new ApiVersion(1, 0); // Sets the default API version to 1.0.
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Reads the version from the URL (e.g., /v1/resource).
});

// Configuring the API version explorer
// This helps Swagger generate documentation for each API version.
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formats the version as "v1", "v2", etc.
    options.SubstituteApiVersionInUrl = true; // Replaces placeholders in the URL with the actual version.
});

// Setting up Swagger for API documentation
// Swagger is a tool that generates interactive API documentation, making it easier to test and understand the API.
builder.Services.AddEndpointsApiExplorer();

// Adds an HTTP context accessor to access the current HTTP context, useful for dynamic server URLs in Swagger.
builder.Services.AddHttpContextAccessor(); 

builder.Services.AddSwaggerGen(options =>
{
    // Dynamically generate Swagger documentation for each API version
    using var serviceProvider = builder.Services.BuildServiceProvider();
    var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        // Create a Swagger document for each version of the API
        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"My Backstage API {description.ApiVersion}", // Title includes the API version.
            Version = description.ApiVersion.ToString(), // Version number.
            Description = $"This is the documentation for API version {description.ApiVersion}.", // Description of the version.
            Contact = new OpenApiContact
            {
                Name = "Support Team", // Contact name for support.
                Email = "support@example.com", // Contact email for support.
                Url = new Uri("https://example.com/support") // Support URL.
            }
        });
    }

    // Add custom filters to handle versioning in Swagger
    options.OperationFilter<RemoveVersionFromParameter>(); // Removes the "version" parameter from the query string.
    options.DocumentFilter<ReplaceVersionWithExactValueInPath>(); // Replaces "v{version}" in paths with the actual version.

    // Add a server URL to the Swagger configuration
    options.AddServer(new OpenApiServer
    {
        Url = $"{builder.Configuration["Swagger:BaseUrl"] ?? "http://localhost:5036"}", // Base URL for the API.
        Description = "Dynamic server URL" // Description of the server.
    });

    // Include XML comments in Swagger documentation if the XML file exists
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath); // Adds detailed comments from the XML file to the documentation.
    }

    options.OperationFilter<DynamicServerOperationFilter>(); // Adds a filter to dynamically set the server URL based on the current HTTP context.
});

var app = builder.Build();

// Applying the CORS policy
// This ensures that the API accepts requests from any origin, as defined earlier.
app.UseCors("AllowAllOrigins");

// Configuring the HTTP request pipeline
// This configures how the application handles incoming requests.
app.UseRouting(); // Enables routing to match incoming requests to endpoints.
app.UseAuthorization(); // Enables authorization middleware to secure endpoints.

// Configuring Swagger
// Swagger is only enabled in development to avoid exposing documentation in production.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enables the Swagger middleware to serve the generated documentation.

    app.UseSwaggerUI(options =>
    {
        // Dynamically add Swagger UI endpoints for each API version
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"My Backstage API {description.ApiVersion}");

            // Add descriptions for each API version
            options.DocumentTitle = $"My Backstage API {description.ApiVersion}";
        }
    });
}

// Global exception handling middleware
// This middleware catches unhandled exceptions and returns a generic error response.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
    });
});

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS for secure communication.
app.MapControllers(); // Map controller endpoints to the routing system.
app.Run(); // Start the application and begin listening for incoming requests.

// Swagger filters to handle API versioning
public class RemoveVersionFromParameter : IOperationFilter
{
    // This method removes the "version" parameter from the Swagger UI operations.
    // It ensures that the API version is not displayed as a query parameter in the Swagger documentation.
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Find the "version" parameter in the operation's parameters list
        var versionParameter = operation.Parameters?.SingleOrDefault(p => p.Name == "version");
        if (versionParameter != null)
        {
            // Remove the "version" parameter if it exists
            operation.Parameters.Remove(versionParameter);
        }
    }
}

public class ReplaceVersionWithExactValueInPath : IDocumentFilter
{
    // This method modifies the Swagger documentation to replace the placeholder "v{version}" in API paths
    // with the actual version of the API. This ensures that the paths in the Swagger UI are accurate
    // and reflect the correct version of the API.
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Create a new collection to store the updated paths.
        var paths = new OpenApiPaths();

        // Loop through all the paths in the Swagger document.
        foreach (var (key, value) in swaggerDoc.Paths)
        {
            // Replace the placeholder "v{version}" in the path with the actual version from the Swagger document's info.
            // For example, if the path is "/api/v{version}/resource" and the version is "1.0",
            // it will be replaced with "/api/v1.0/resource".
            paths.Add(key.Replace("v{version}", swaggerDoc.Info.Version), value);
        }

        // Update the Swagger document's paths with the modified paths.
        swaggerDoc.Paths = paths;
    }
}
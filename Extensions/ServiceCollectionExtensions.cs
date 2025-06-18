// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace MyBackstageAPI.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CORS policy to allow requests from any origin, header, and method.
    /// This is useful for development and testing purposes, but should be restricted in production.
    /// </summary>
    /// <param name="services"> The service collection to add the CORS policy to.</param>
    /// <param name="configuration"> The configuration object to read settings from, if needed.</param>
    /// <returns> The updated service collection with CORS policy added.</returns>
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
        return services;
    }

    // public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //         .AddJwtBearer(options =>
    //         {
    //             options.TokenValidationParameters = new TokenValidationParameters
    //             {
    //                 ValidateIssuer = true,
    //                 ValidateAudience = true,
    //                 ValidateLifetime = true,
    //                 ValidateIssuerSigningKey = true,
    //                 ValidIssuer = configuration["Jwt:Issuer"],
    //                 ValidAudience = configuration["Jwt:Audience"],
    //                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
    //             };
    //         });
    //     return services;
    // }

    public static IServiceCollection AddApiVersioningWithExplorer(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddSwaggerWithXmlCommentsAndVersioning(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
            options.OperationFilter<RemoveVersionFromParameter>();
            options.DocumentFilter<ReplaceVersionWithExactValueInPath>();
            options.OperationFilter<DynamicServerOperationFilter>();
        });
        return services;
    }
}
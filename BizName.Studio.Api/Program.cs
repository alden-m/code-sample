using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using BizName.Studio.Api;
using BizName.Studio.Api.Authorization;
using BizName.Studio.Api.Extensions;
using BizName.Studio.Api.Middlewares;
using BizName.Studio.App.Configurator;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Experiences;
using BizName.Studio.Data.Database;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Use Serilog for logging
builder.Host.UseSerilog();

try
{
    Log.Information("Starting BizName Studio API");

// CDN functionality removed for sanitization

builder.AddB2CAuthentication();

builder.Services.AddRequestContext();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
ServiceConfiguratorInvoker.RegisterAll(builder.Services, builder.Configuration);

// Add OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "BizName.Studio.Api.xml"));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "BizName.Studio.App.xml"));

    // Enable polymorphic documentation
    options.UseOneOfForPolymorphism();
    options.SelectDiscriminatorNameUsing(type =>
    {
        if (typeof(IExperienceAction).IsAssignableFrom(type))
            return Constants.Experience.ActionTypeDiscriminator;
        if (typeof(IExperienceCondition).IsAssignableFrom(type))
            return Constants.Experience.ConditionTypeDiscriminator;
        return null;
    });

    // Add JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    // TODO: Limit the policy to the exact endpoints that we want to allow for public and required by Sona mode  
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.WithOrigins(
                      "https://localhost:7200", // UI
                      "https://localhost:7300", // Chatbot API
                      "https://localhost:7000", // CDN
                      "http://localhost:3000",  // Development
                      "http://127.0.0.1:3000"   // Development
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Needed for authenticated requests
        });
});

// Add HTTP request logging
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod | 
                          HttpLoggingFields.RequestPath | 
                          HttpLoggingFields.ResponseStatusCode | 
                          HttpLoggingFields.Duration;
    options.RequestBodyLogLimit = 0; // Don't log request bodies for performance
    options.ResponseBodyLogLimit = 0; // Don't log response bodies for performance
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.IncludeFields = true; // Include fields during serialization
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false; // Enforce case-sensitive property names
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Use camelCase for API responses
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Serialize enums as strings
});

var app = builder.Build();

// Initialize database and containers
using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ApiLoggingMiddleware>();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseMultitenant();

app.MapControllers();

app.MapGet("/", [AllowAnonymous] () => "Service is up and running");

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible for integration tests
public partial class Program { }

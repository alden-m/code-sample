using System.Text.Json;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Api.Middlewares;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions
/// and converts them to consistent ErrorResponse format with proper logging
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred during request {Method} {Path}. TraceId: {TraceId}", 
                context.Request.Method, 
                context.Request.Path, 
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorResponse = new ErrorResponse
        {
            Error = "InternalServerError",
            Message = "An unexpected error occurred",
            StatusCode = 500,
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}

using System.Diagnostics;

namespace BizName.Studio.Api.Middlewares;

public class ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        
        logger.LogInformation("API {Method} {Path} from {RemoteIp}", context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress);
        
        await next(context);
        
        sw.Stop();
        logger.LogInformation("API {Method} {Path} -> {StatusCode} in {ElapsedMs}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}

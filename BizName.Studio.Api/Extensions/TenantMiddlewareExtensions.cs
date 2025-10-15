using BizName.Studio.Api.Middlewares;

namespace BizName.Studio.Api.Extensions;

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseMultitenant(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseMiddleware<TenantValidationMiddleware>();
        app.UseMiddleware<TenantProvisioningMiddleware>();

        return app;
    }
}

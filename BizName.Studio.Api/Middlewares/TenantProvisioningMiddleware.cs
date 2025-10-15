using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Tenants;

namespace BizName.Studio.Api.Middlewares;

public class TenantProvisioningMiddleware(RequestDelegate next, ILogger<TenantProvisioningMiddleware> logger)
{
    public const string OrganizationClaimName = "extension_Organization";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await next(context);
            return;
        }

        var organizationIdClaim = context.User?.FindFirst(OrganizationClaimName)?.Value;

        if (string.IsNullOrEmpty(organizationIdClaim) || !Guid.TryParse(organizationIdClaim, out var organizationId))
        {
            logger.LogWarning("Tenant ID claim is missing.");
            
            var errorResponse = new ErrorResponse
            {
                Error = "Unauthorized",
                Message = "Identification info for the current subscription is needed.",
                StatusCode = 403,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
            return;
        }

        var result = await tenantService.EnsureProvisioning(Tenant.New(organizationId, "Default Organization"));
        if (!result.Success) throw new InvalidOperationException("Couldn't provision tenant");
        
        await next(context);
    }
}

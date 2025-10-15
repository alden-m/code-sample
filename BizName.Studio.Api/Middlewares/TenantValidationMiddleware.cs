using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Api.Middlewares;

public class TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
{
    public const string OrganizationClaimName = "extension_Organization";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await next(context);
            return;
        }

        // Note: Azure B2C takes care of validating legitimacy of token, we just ensure it has the right claims
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

        await next(context);
    }
}

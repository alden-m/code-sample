using System.Security.Claims;
using BizName.Studio.Api.Middlewares;

namespace BizName.Studio.Api;

public interface IRequestContext
{
    //todo: this exists  in 3 places, unify it somewhere central.
    public const string OrganizationClaimName = "extension_Organization";

    private const string UserNameClaimName = "name";

    public ClaimsPrincipal User { get; }

    public Guid TenantId
    {
        get
        {
            var organizationClaim = User?.FindFirst(OrganizationClaimName);
            return Guid.TryParse(organizationClaim?.Value, out var organizationId) ? organizationId : Guid.Empty;
        }
    }

    public string UserName => User?.Claims?.FirstOrDefault(c => c.Type == UserNameClaimName)?.Value ?? "unknown user";
}

public class RequestContext(IHttpContextAccessor httpContextAccessor) : IRequestContext
{
    public ClaimsPrincipal User => httpContextAccessor?.HttpContext?.User;
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequestContext(this IServiceCollection services)
    {
        services.AddScoped<IRequestContext, RequestContext>();

        return services;
    }
}

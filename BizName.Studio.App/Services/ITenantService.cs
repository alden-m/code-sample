using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Tenants;

namespace BizName.Studio.App.Services;

public interface ITenantService
{
    Task<OperationResult> EnsureProvisioning(Tenant tenant);
    Task<OperationResult<Tenant>> GetById(Guid tenantId);
    Task<OperationResult> DeleteTenant(Guid tenantId);
}

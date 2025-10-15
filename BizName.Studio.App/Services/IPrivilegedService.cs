using BizName.Studio.Contracts.Common;

namespace BizName.Studio.App.Services;

/// <summary>
/// Cross-tenant privileged operations
/// </summary>
public interface IPrivilegedService
{
    Task<OperationResult<Guid>> GetTenantIdByWebsiteIdAsync<T>(Guid websiteId);
}

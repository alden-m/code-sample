using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.Services;

public interface IWebsiteService
{
    Task<OperationResult<IEnumerable<Website>>> ListWebsites(Guid tenantId);
    Task<OperationResult<Guid>> CreateWebsite(Guid tenantId, Website website);
    Task<OperationResult<Website>> GetWebsiteById(Guid tenantId, Guid websiteId);
    Task<OperationResult<Website>> UpdateWebsite(Guid tenantId, Website website);
    Task<OperationResult> DeleteWebsite(Guid tenantId, Guid websiteId);
}

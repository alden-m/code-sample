using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Inventory;

namespace BizName.Studio.App.Services;

public interface IAssetService
{
    Task<OperationResult<Guid>> CreateAssetAsync(Asset asset);
    Task<OperationResult<Asset>> GetAssetAsync(Guid assetId);
    Task<OperationResult<List<Asset>>> ListAssetsAsync(Guid tenantId, AssetCategory? category = null);
    Task<OperationResult<Asset>> UpdateAssetAsync(Asset asset);
    Task<OperationResult> DeleteAssetAsync(Guid assetId);
    Task<OperationResult<decimal>> CalculateAssetValueAsync(Guid assetId);
    Task<OperationResult<List<Asset>>> GetAssetHierarchyAsync(Guid parentAssetId);
}
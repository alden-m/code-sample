using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Analytics;

namespace BizName.Studio.App.Services;

public interface IMetricsService
{
    Task<OperationResult<Guid>> CreateCollectorAsync(MetricCollector collector);
    Task<OperationResult<MetricCollector>> GetCollectorAsync(Guid collectorId);
    Task<OperationResult<List<MetricCollector>>> ListCollectorsAsync(Guid tenantId);
    Task<OperationResult> ExecuteCollectorAsync(Guid collectorId);
    Task<OperationResult<Dictionary<string, object>>> GetMetricsDataAsync(Guid collectorId, DateTime from, DateTime to);
    Task<OperationResult> UpdateCollectorConfigurationAsync(Guid collectorId, Dictionary<string, object> parameters);
}
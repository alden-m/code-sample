using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Analytics;

public class MetricCollector : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CollectorName { get; set; } = string.Empty;
    public MetricType Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public TimeSpan SamplingInterval { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastExecuted { get; set; }
}

public enum MetricType
{
    Performance,
    Memory,
    Network,
    Storage,
    Custom
}
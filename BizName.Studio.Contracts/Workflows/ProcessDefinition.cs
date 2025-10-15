using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Workflows;

public class ProcessDefinition : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProcessName { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public List<ProcessStep> Steps { get; set; } = new();
    public ProcessStatus Status { get; set; }
    public Dictionary<string, string> Variables { get; set; } = new();
    public TimeSpan ExecutionTimeout { get; set; }
}

public class ProcessStep
{
    public string StepId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
    public List<string> NextSteps { get; set; } = new();
    public bool IsParallel { get; set; }
}

public enum ProcessStatus
{
    Draft,
    Active,
    Suspended,
    Completed,
    Failed
}
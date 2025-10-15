namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ExperienceResponse
{
    public Guid Id { get; set; }
    public Guid SourceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<object>? Rules { get; set; }
    public List<object>? Transformations { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string> Configuration { get; set; } = new();
}

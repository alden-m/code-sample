namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ExperienceResponse
{
    public Guid Id { get; set; }
    public Guid WebsiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<object>? Conditions { get; set; }
    public List<object>? Actions { get; set; }
    public bool IsPublished { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class WebsiteResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
}

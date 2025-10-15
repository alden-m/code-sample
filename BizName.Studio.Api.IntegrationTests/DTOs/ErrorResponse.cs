namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string? TraceId { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}

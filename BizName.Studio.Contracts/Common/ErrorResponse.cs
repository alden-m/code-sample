using System.Text.Json.Serialization;

namespace BizName.Studio.Contracts.Common;

/// <summary>
/// Standardized error response model for all API endpoints.
/// Provides consistent error information to clients.
/// </summary>
public record ErrorResponse
{
    /// <summary>
    /// Error type classification (e.g., "ValidationError", "NotFound").
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; init; } = "";

    /// <summary>
    /// Human-readable error message describing what went wrong.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = "";

    /// <summary>
    /// HTTP status code for this error.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    /// <summary>
    /// Timestamp when the error occurred.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Unique identifier for this request, useful for debugging and support.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }

    /// <summary>
    /// Field-specific validation errors (for 400 Bad Request responses).
    /// Key is the field name, value is array of error messages for that field.
    /// </summary>
    [JsonPropertyName("validationErrors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    /// <summary>
    /// Additional context or metadata about the error.
    /// </summary>
    [JsonPropertyName("details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Details { get; init; }
}

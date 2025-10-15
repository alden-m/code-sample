namespace BizName.Studio.Contracts.Common;

/// <summary>
/// Defines the types of errors that can occur during operation execution.
/// These map directly to appropriate HTTP status codes in the API layer.
/// </summary>
public enum OperationErrorType
{
    /// <summary>
    /// Client sent invalid data that fails validation rules.
    /// Maps to HTTP 400 Bad Request.
    /// Examples: Empty required fields, invalid format, constraint violations, duplicate names, business rule violations.
    /// </summary>
    ValidationError,

    /// <summary>
    /// Requested resource was not found.
    /// Maps to HTTP 404 Not Found.
    /// Examples: Entity with given ID doesn't exist, tenant doesn't have access.
    /// </summary>
    NotFound,

    /// <summary>
    /// Internal server error that is not the client's fault.
    /// Maps to HTTP 500 Internal Server Error.
    /// Examples: Database connection failures, unexpected exceptions, external service failures.
    /// </summary>
    InternalError
}

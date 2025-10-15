using Microsoft.AspNetCore.Mvc;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Api.Extensions;

/// <summary>
/// Extension methods to convert OperationResult to appropriate HTTP responses.
/// Ensures consistent status codes and error format across all endpoints.
/// </summary>
public static class OperationResultExtensions
{
    /// <summary>
    /// Converts an OperationResult to an ErrorResponse IActionResult for sad path only.
    /// Use this for explicit happy/sad path handling in controllers.
    /// </summary>
    /// <typeparam name="T">The data type contained in the operation result</typeparam>
    /// <param name="result">The operation result to convert (must be unsuccessful)</param>
    /// <param name="httpContext">The HTTP context for trace ID</param>
    /// <returns>IActionResult with appropriate error status code and ErrorResponse body</returns>
    public static IActionResult ToErrorResponse<T>(this OperationResult<T> result, HttpContext httpContext)
    {
        if (result.Success)
            throw new InvalidOperationException("ToErrorResponse should only be called for unsuccessful operations. Use manual return for happy path.");

        var errorResponse = new ErrorResponse
        {
            Message = result.ErrorMessage,
            TraceId = httpContext.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        return result.ErrorType switch
        {
            OperationErrorType.ValidationError => new BadRequestObjectResult(errorResponse with 
            { 
                Error = "ValidationError",
                StatusCode = 400,
                ValidationErrors = result.Metadata?.ContainsKey(OperationResultMetaDataKeys.ValidationErrors) == true 
                    ? result.Metadata[OperationResultMetaDataKeys.ValidationErrors] as Dictionary<string, string[]>
                    : null
            }),
            
            OperationErrorType.NotFound => new NotFoundObjectResult(errorResponse with 
            { 
                Error = "NotFound",
                StatusCode = 404 
            }),
            
            _ => new ObjectResult(errorResponse with 
            { 
                Error = "InternalServerError",
                StatusCode = 500 
            })
            { StatusCode = 500 }
        };
    }

    /// <summary>
    /// Converts a non-generic OperationResult to an ErrorResponse IActionResult for sad path only.
    /// </summary>
    /// <param name="result">The operation result to convert (must be unsuccessful)</param>
    /// <param name="httpContext">The HTTP context for trace ID</param>
    /// <returns>IActionResult with appropriate error status code and ErrorResponse body</returns>
    public static IActionResult ToErrorResponse(this OperationResult result, HttpContext httpContext)
    {
        if (result.Success)
            throw new InvalidOperationException("ToErrorResponse should only be called for unsuccessful operations. Use manual return for happy path.");

        var errorResponse = new ErrorResponse
        {
            Message = result.ErrorMessage,
            TraceId = httpContext.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        return result.ErrorType switch
        {
            OperationErrorType.ValidationError => new BadRequestObjectResult(errorResponse with 
            { 
                Error = "ValidationError",
                StatusCode = 400,
                ValidationErrors = result.Metadata?.ContainsKey(OperationResultMetaDataKeys.ValidationErrors) == true 
                    ? result.Metadata[OperationResultMetaDataKeys.ValidationErrors] as Dictionary<string, string[]>
                    : null
            }),
            
            OperationErrorType.NotFound => new NotFoundObjectResult(errorResponse with 
            { 
                Error = "NotFound",
                StatusCode = 404 
            }),
            
            _ => new ObjectResult(errorResponse with 
            { 
                Error = "InternalServerError",
                StatusCode = 500 
            })
            { StatusCode = 500 }
        };
    }

}

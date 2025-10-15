using FluentValidation.Results;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Extensions;

public static class ValidationResultExtensions
{
    public static OperationResult ToOperationResult(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return OperationResult.SuccessResult();
        }

        var fieldErrors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
        
        var metadata = new Dictionary<string, object>
        {
            [OperationResultMetaDataKeys.ValidationErrors] = fieldErrors
        };
        
        return OperationResult.ValidationFailure("Validation failed", metadata);
    }

    public static OperationResult<T> ToOperationResult<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return OperationResult<T>.SuccessResult(default(T)!);
        }

        var fieldErrors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
        
        var metadata = new Dictionary<string, object>
        {
            [OperationResultMetaDataKeys.ValidationErrors] = fieldErrors
        };
        
        return OperationResult<T>.ValidationFailure("Validation failed", metadata);
    }

    public static IEnumerable<string> GetErrorMessages(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(e => e.ErrorMessage);
    }
}

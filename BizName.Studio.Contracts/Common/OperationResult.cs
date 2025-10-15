namespace BizName.Studio.Contracts.Common;

public static class OperationResultMetaDataKeys
{
    public const string ValidationErrors = "ValidationErrors";
}

public class OperationResult
{
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; }
    public OperationErrorType? ErrorType { get; private set; }
    public Dictionary<string, object>? Metadata { get; private set; }

    private OperationResult(bool success, string errorMessage, OperationErrorType? errorType = null, Dictionary<string, object>? metadata = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        Metadata = metadata;
    }

    public static OperationResult SuccessResult()
    {
        return new OperationResult(true, string.Empty);
    }


    public static OperationResult ValidationFailure(string errorMessage, Dictionary<string, object> metadata = null)
    {
        return new OperationResult(false, errorMessage, OperationErrorType.ValidationError, metadata);
    }

    public static OperationResult NotFoundFailure(string errorMessage)
    {
        return new OperationResult(false, errorMessage, OperationErrorType.NotFound);
    }


    public static OperationResult InternalFailure(string errorMessage)
    {
        return new OperationResult(false, errorMessage, OperationErrorType.InternalError);
    }
}

public class OperationResult<T>
{
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; }
    public OperationErrorType? ErrorType { get; private set; }
    public T? Data { get; private set; }
    public Dictionary<string, object>? Metadata { get; private set; }

    private OperationResult(bool success, string errorMessage, OperationErrorType? errorType, T? data, Dictionary<string, object>? metadata = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        Data = data;
        Metadata = metadata;
    }

    public static OperationResult<T> SuccessResult(T data)
    {
        return new OperationResult<T>(true, string.Empty, null, data);
    }

    public static OperationResult<T> ValidationFailure(string errorMessage, Dictionary<string, object> metadata = null)
    {
        return new OperationResult<T>(false, errorMessage, OperationErrorType.ValidationError, default, metadata);
    }

    public static OperationResult<T> NotFoundFailure(string errorMessage)
    {
        return new OperationResult<T>(false, errorMessage, OperationErrorType.NotFound, default);
    }


    public static OperationResult<T> InternalFailure(string errorMessage)
    {
        return new OperationResult<T>(false, errorMessage, OperationErrorType.InternalError, default);
    }
}

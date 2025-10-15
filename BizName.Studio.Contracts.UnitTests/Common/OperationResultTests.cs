using FluentAssertions;
using BizName.Studio.Contracts.Common;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Common;

public class OperationResultTests
{
    [Fact]
    public void SuccessResult_ShouldCreateSuccessfulOperationResult()
    {
        // Act
        var result = OperationResult.SuccessResult();

        // Assert
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeEmpty();
        result.ErrorType.Should().BeNull();
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void ValidationFailure_WithMessage_ShouldCreateValidationErrorResult()
    {
        // Arrange
        var errorMessage = "Validation failed";

        // Act
        var result = OperationResult.ValidationFailure(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void ValidationFailure_WithMessageAndMetadata_ShouldCreateValidationErrorResultWithMetadata()
    {
        // Arrange
        var errorMessage = "Validation failed";
        var metadata = new Dictionary<string, object>
        {
            { "Field1", "Error1" },
            { "Field2", "Error2" }
        };

        // Act
        var result = OperationResult.ValidationFailure(errorMessage, metadata);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.Metadata.Should().BeEquivalentTo(metadata);
    }

    [Fact]
    public void NotFoundFailure_ShouldCreateNotFoundErrorResult()
    {
        // Arrange
        var errorMessage = "Resource not found";

        // Act
        var result = OperationResult.NotFoundFailure(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void InternalFailure_ShouldCreateInternalErrorResult()
    {
        // Arrange
        var errorMessage = "Internal server error";

        // Act
        var result = OperationResult.InternalFailure(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.InternalError);
        result.Metadata.Should().BeNull();
    }
}

public class OperationResultGenericTests
{
    [Fact]
    public void SuccessResult_WithData_ShouldCreateSuccessfulOperationResultWithData()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = OperationResult<string>.SuccessResult(data);

        // Assert
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeEmpty();
        result.ErrorType.Should().BeNull();
        result.Data.Should().Be(data);
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void SuccessResult_WithComplexData_ShouldCreateSuccessfulOperationResultWithComplexData()
    {
        // Arrange
        var data = new { Id = 1, Name = "Test" };

        // Act
        var result = OperationResult<object>.SuccessResult(data);

        // Assert
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeEmpty();
        result.ErrorType.Should().BeNull();
        result.Data.Should().BeEquivalentTo(data);
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void ValidationFailure_WithMessage_ShouldCreateValidationErrorResultWithDefaultData()
    {
        // Arrange
        var errorMessage = "Validation failed";

        // Act
        var result = OperationResult<string>.ValidationFailure(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.Data.Should().BeNull();
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void ValidationFailure_WithMessageAndMetadata_ShouldCreateValidationErrorResultWithMetadata()
    {
        // Arrange
        var errorMessage = "Validation failed";
        var metadata = new Dictionary<string, object>
        {
            { OperationResultMetaDataKeys.ValidationErrors, new[] { "Field is required" } }
        };

        // Act
        var result = OperationResult<int>.ValidationFailure(errorMessage, metadata);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.Data.Should().Be(0);
        result.Metadata.Should().BeEquivalentTo(metadata);
    }

    [Fact]
    public void NotFoundFailure_ShouldCreateNotFoundErrorResultWithDefaultData()
    {
        // Arrange
        var errorMessage = "Resource not found";

        // Act
        var result = OperationResult<string>.NotFoundFailure(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
        result.Data.Should().BeNull();
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void InternalFailure_ShouldCreateInternalErrorResultWithDefaultData()
    {
        // Arrange
        var errorMessage = "Internal server error";

        // Act
        var result = OperationResult<string>.InternalFailure(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.InternalError);
        result.Data.Should().BeNull();
        result.Metadata.Should().BeNull();
    }

    [Fact]
    public void ValidationFailure_WithValidationErrorsMetadata_ShouldUseMetadataConstants()
    {
        // Arrange
        var errorMessage = "Multiple validation errors";
        var validationErrors = new[] { "Name is required", "Email format is invalid" };
        var metadata = new Dictionary<string, object>
        {
            { OperationResultMetaDataKeys.ValidationErrors, validationErrors }
        };

        // Act
        var result = OperationResult<object>.ValidationFailure(errorMessage, metadata);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.Metadata.Should().ContainKey(OperationResultMetaDataKeys.ValidationErrors);
        result.Metadata![OperationResultMetaDataKeys.ValidationErrors].Should().BeEquivalentTo(validationErrors);
    }
}

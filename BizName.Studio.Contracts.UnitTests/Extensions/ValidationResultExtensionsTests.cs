using FluentValidation.Results;
using FluentAssertions;
using BizName.Studio.Contracts.Extensions;
using BizName.Studio.Contracts.Common;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Extensions;

public class ValidationResultExtensionsTests
{
    [Fact]
    public void ToOperationResult_WithValidResult_Should_ReturnSuccess()
    {
        // Arrange
        var validationResult = new ValidationResult();
        
        // Act
        var operationResult = validationResult.ToOperationResult();
        
        // Assert
        operationResult.Success.Should().BeTrue();
        operationResult.ErrorMessage.Should().BeEmpty();
        operationResult.ErrorType.Should().BeNull();
        operationResult.Metadata.Should().BeNull();
    }

    [Fact]
    public void ToOperationResult_WithInvalidResult_Should_ReturnValidationFailure()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid"),
            new ValidationFailure("Name", "Name must be longer than 2 characters")
        });
        
        // Act
        var operationResult = validationResult.ToOperationResult();
        
        // Assert
        operationResult.Success.Should().BeFalse();
        operationResult.ErrorMessage.Should().Be("Validation failed");
        operationResult.ErrorType.Should().Be(OperationErrorType.ValidationError);
        operationResult.Metadata.Should().NotBeNull();
        
        var validationErrors = operationResult.Metadata![OperationResultMetaDataKeys.ValidationErrors] as Dictionary<string, string[]>;
        validationErrors.Should().NotBeNull();
        validationErrors!["Name"].Should().HaveCount(2);
        validationErrors["Name"].Should().Contain("Name is required");
        validationErrors["Name"].Should().Contain("Name must be longer than 2 characters");
        validationErrors["Email"].Should().HaveCount(1);
        validationErrors["Email"].Should().Contain("Email is invalid");
    }

    [Fact]
    public void ToOperationResult_Generic_WithValidResult_Should_ReturnSuccess()
    {
        // Arrange
        var validationResult = new ValidationResult();
        
        // Act
        var operationResult = validationResult.ToOperationResult<string>();
        
        // Assert
        operationResult.Success.Should().BeTrue();
        operationResult.ErrorMessage.Should().BeEmpty();
        operationResult.ErrorType.Should().BeNull();
        operationResult.Metadata.Should().BeNull();
    }

    [Fact]
    public void ToOperationResult_Generic_WithInvalidResult_Should_ReturnValidationFailure()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Id", "Id is required")
        });
        
        // Act
        var operationResult = validationResult.ToOperationResult<Guid>();
        
        // Assert
        operationResult.Success.Should().BeFalse();
        operationResult.ErrorMessage.Should().Be("Validation failed");
        operationResult.ErrorType.Should().Be(OperationErrorType.ValidationError);
        operationResult.Metadata.Should().NotBeNull();
        
        var validationErrors = operationResult.Metadata![OperationResultMetaDataKeys.ValidationErrors] as Dictionary<string, string[]>;
        validationErrors.Should().NotBeNull();
        validationErrors!["Id"].Should().HaveCount(1);
        validationErrors["Id"].Should().Contain("Id is required");
    }

    [Fact]
    public void GetErrorMessages_Should_ReturnAllErrorMessages()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid"),
            new ValidationFailure("Age", "Age must be positive")
        });
        
        // Act
        var errorMessages = validationResult.GetErrorMessages().ToList();
        
        // Assert
        errorMessages.Should().HaveCount(3);
        errorMessages.Should().Contain("Name is required");
        errorMessages.Should().Contain("Email is invalid");
        errorMessages.Should().Contain("Age must be positive");
    }

    [Fact]
    public void GetErrorMessages_WithValidResult_Should_ReturnEmpty()
    {
        // Arrange
        var validationResult = new ValidationResult();
        
        // Act
        var errorMessages = validationResult.GetErrorMessages().ToList();
        
        // Assert
        errorMessages.Should().BeEmpty();
    }
}

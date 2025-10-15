using FluentValidation;
using FluentValidation.Results;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services;
using BizName.Studio.App.Services.Impl;
using BizName.Studio.Contracts.Websites;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace BizName.Studio.App.UnitTests.Services;

public class ExperienceServiceImplUpdateTests
{
    private readonly IPartitionedRepository<Experience> _mockExpRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IWebsiteService _mockWebsiteService = Substitute.For<IWebsiteService>();
    private readonly IValidator<Experience> _mockValidator = Substitute.For<IValidator<Experience>>();
    private readonly ExperienceServiceImpl _service;

    public ExperienceServiceImplUpdateTests()
    {
        _service = new ExperienceServiceImpl(_mockExpRepo, _mockWebsiteService, _mockValidator, NullLogger<ExperienceServiceImpl>.Instance);
    }

    [Fact]
    public async Task UpdateExperience_ExperienceNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();
        var updatedExperience = TestData.NewExperience;
        _mockExpRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns((Experience)null);

        // Act
        var result = await _service.UpdateExperience(tenantId, experienceId, updatedExperience);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateExperience_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();
        var existingExperience = TestData.NewExperience;
        var updatedExperience = TestData.NewExperience;
        var validationResult = new FluentValidation.Results.ValidationResult([new ValidationFailure("Name", "Name is required")]);
        
        _mockExpRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(existingExperience);
        _mockValidator.ValidateAsync(updatedExperience).Returns(validationResult);

        // Act
        var result = await _service.UpdateExperience(tenantId, experienceId, updatedExperience);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
    }

    [Fact]
    public async Task UpdateExperience_WebsiteIdChanged_ValidatesNewWebsite()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();
        var existingExperience = TestData.NewExperience;
        var updatedExperience = TestData.NewExperience;
        var newWebsiteId = Guid.NewGuid();
        updatedExperience.WebsiteId = newWebsiteId;
        
        _mockExpRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(existingExperience);
        _mockValidator.ValidateAsync(updatedExperience).Returns(new FluentValidation.Results.ValidationResult());
        _mockWebsiteService.GetWebsiteById(tenantId, newWebsiteId)
            .Returns(OperationResult<Website>.NotFoundFailure("Website not found"));

        // Act
        var result = await _service.UpdateExperience(tenantId, experienceId, updatedExperience);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
    }

    [Fact]
    public async Task UpdateExperience_Success_UpdatesAllPropertiesComprehensively()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();
        var originalId = Guid.NewGuid();
        
        // Get two separate instances with different values
        var existingExperience = TestData.NewExperience;
        existingExperience.Id = originalId; // Set original ID
        
        var updatedExperience = TestData.NewExperience; // Get a fresh instance with different random values
        updatedExperience.Id = originalId; // Set original ID
        updatedExperience.WebsiteId = existingExperience.WebsiteId; // Keep same website to avoid validation
        
        _mockExpRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(existingExperience);
        _mockValidator.ValidateAsync(updatedExperience).Returns(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _service.UpdateExperience(tenantId, experienceId, updatedExperience);

        // Assert
        result.Success.Should().BeTrue();
        
        // Use reflection to verify ALL properties are copied (except Id)
        var properties = typeof(Experience).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            
        foreach (var property in properties)
        {
            var existingValue = property.GetValue(existingExperience);
            var updatedValue = property.GetValue(updatedExperience);
            
            existingValue.Should().BeEquivalentTo(updatedValue, 
                $"Property '{property.Name}' should be updated but was not copied from updatedExperience to existingExperience");
        }
        
        // Ensure ID is NOT updated
        existingExperience.Id.Should().Be(originalId, "ID should never be updated");
        
        await _mockExpRepo.Received(1).UpdateAsync(tenantId, existingExperience);
    }
}

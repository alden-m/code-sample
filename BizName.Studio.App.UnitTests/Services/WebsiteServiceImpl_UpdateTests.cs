using FluentValidation;
using FluentValidation.Results;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using BizName.Studio.Contracts.Websites;
using System.Reflection;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace BizName.Studio.App.UnitTests.Services;

public class WebsiteServiceImplUpdateTests
{
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IPartitionedRepository<Experience> _mockExperienceRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IValidator<Website> _mockValidator = Substitute.For<IValidator<Website>>();
    private readonly WebsiteServiceImpl _service;

    public WebsiteServiceImplUpdateTests()
    {
        _service = new WebsiteServiceImpl(_mockWebsiteRepo, _mockExperienceRepo, _mockValidator, NullLogger<WebsiteServiceImpl>.Instance);
    }

    [Fact]
    public async Task UpdateWebsite_EmptyTenantId_ThrowsArgumentException()
    {
        // Arrange
        var emptyTenantId = Guid.Empty;
        var website = TestData.NewWebsite;
        var act = () => _service.UpdateWebsite(emptyTenantId, website);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("TenantId cannot be empty*");
    }

    [Fact]
    public async Task UpdateWebsite_NullWebsite_ThrowsArgumentNullException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var act = () => _service.UpdateWebsite(tenantId, null);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("website");
    }

    [Fact]
    public async Task UpdateWebsite_WebsiteNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var website = TestData.NewWebsite;
        _mockWebsiteRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns((Website)null);

        // Act
        var result = await _service.UpdateWebsite(tenantId, website);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateWebsite_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var existingWebsite = TestData.NewWebsite;
        var updatedWebsite = TestData.NewWebsite;
        _mockWebsiteRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns(existingWebsite);
        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);
        _mockValidator.ValidateAsync(updatedWebsite).Returns(validationResult);

        // Act
        var result = await _service.UpdateWebsite(tenantId, updatedWebsite);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
    }

    [Fact]
    public async Task UpdateWebsite_Success_UpdatesAllPropertiesComprehensively()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var originalId = Guid.NewGuid();
        
        // Get two separate instances with different values
        var existingWebsite = TestData.NewWebsite;
        existingWebsite.Id = originalId; // Set original ID
        
        var updatedWebsite = TestData.NewWebsite; // Get a fresh instance with different random values
        updatedWebsite.Id = originalId; // Set original ID
        
        _mockWebsiteRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns(existingWebsite);
        _mockValidator.ValidateAsync(updatedWebsite).Returns(new ValidationResult());

        // Act
        var result = await _service.UpdateWebsite(tenantId, updatedWebsite);

        // Assert
        result.Success.Should().BeTrue();
        
        // Use reflection to verify ALL properties are copied (except Id)
        var properties = typeof(Website).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            
        foreach (var property in properties)
        {
            var existingValue = property.GetValue(existingWebsite);
            var updatedValue = property.GetValue(updatedWebsite);
            
            existingValue.Should().BeEquivalentTo(updatedValue, 
                $"Property '{property.Name}' should be updated but was not copied from updatedWebsite to existingWebsite");
        }
        
        // Ensure ID is NOT updated
        existingWebsite.Id.Should().Be(originalId, "ID should never be updated");
        
        await _mockWebsiteRepo.Received(1).UpdateAsync(tenantId, existingWebsite);
    }
}

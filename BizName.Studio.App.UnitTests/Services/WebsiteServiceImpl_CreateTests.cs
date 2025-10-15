using FluentValidation;
using FluentValidation.Results;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using BizName.Studio.Contracts.Websites;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace BizName.Studio.App.UnitTests.Services;

public class WebsiteServiceImplCreateTests
{
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IPartitionedRepository<Experience> _mockExperienceRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IValidator<Website> _mockValidator = Substitute.For<IValidator<Website>>();
    private readonly WebsiteServiceImpl _service;

    public WebsiteServiceImplCreateTests()
    {
        _service = new WebsiteServiceImpl(_mockWebsiteRepo, _mockExperienceRepo, _mockValidator, NullLogger<WebsiteServiceImpl>.Instance);
    }

    [Fact]
    public async Task CreateWebsite_EmptyTenantId_ThrowsArgumentException()
    {
        // Arrange
        var emptyTenantId = Guid.Empty;
        var website = TestData.NewWebsite;
        var act = () => _service.CreateWebsite(emptyTenantId, website);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("TenantId cannot be empty*");
    }

    [Fact]
    public async Task CreateWebsite_NullWebsite_ThrowsArgumentNullException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var act = () => _service.CreateWebsite(tenantId, null);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("website");
    }

    [Fact]
    public async Task CreateWebsite_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var website = TestData.NewWebsite;
        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);
        _mockValidator.ValidateAsync(website).Returns(validationResult);

        // Act
        var result = await _service.CreateWebsite(tenantId, website);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
    }

    [Fact]
    public async Task CreateWebsite_DuplicateName_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var website = TestData.NewWebsite;
        _mockValidator.ValidateAsync(website).Returns(new ValidationResult());
        _mockWebsiteRepo.ListAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns(new List<Website> { TestData.NewWebsite });

        // Act
        var result = await _service.CreateWebsite(tenantId, website);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.ErrorMessage.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateWebsite_Success_GeneratesIdAndReturnsSuccess()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var website = TestData.NewWebsite;
        website.Id = Guid.Empty;
        _mockValidator.ValidateAsync(website).Returns(new ValidationResult());
        _mockWebsiteRepo.ListAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns(new List<Website>());

        // Act
        var result = await _service.CreateWebsite(tenantId, website);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBe(Guid.Empty);
        website.Id.Should().NotBe(Guid.Empty);
        await _mockWebsiteRepo.Received(1).AddAsync(tenantId, website);
    }
}

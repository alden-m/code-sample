using FluentValidation;
using FluentValidation.Results;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services;
using BizName.Studio.App.Services.Impl;
using BizName.Studio.Contracts.Websites;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;

namespace BizName.Studio.App.UnitTests.Services;

public class ExperienceServiceImplCreateTests
{
    private readonly IPartitionedRepository<Experience> _mockExpRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IWebsiteService _mockWebsiteService = Substitute.For<IWebsiteService>();
    private readonly IValidator<Experience> _mockValidator = Substitute.For<IValidator<Experience>>();
    private readonly ExperienceServiceImpl _service;

    public ExperienceServiceImplCreateTests()
    {
        _service = new ExperienceServiceImpl(_mockExpRepo, _mockWebsiteService, _mockValidator, NullLogger<ExperienceServiceImpl>.Instance);
    }

    [Fact]
    public async Task CreateExperience_EmptyTenantId_ThrowsArgumentException()
    {
        // Arrange
        var emptyTenantId = Guid.Empty;
        var experience = TestData.NewExperience;

        // Act & Assert
        var act = () => _service.CreateExperience(emptyTenantId, experience);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("TenantId cannot be empty*");
    }

    [Fact]
    public async Task CreateExperience_NullExperience_ThrowsArgumentNullException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var act = () => _service.CreateExperience(tenantId, null);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("experience");
    }

    [Fact]
    public async Task CreateExperience_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experience = TestData.NewExperience;
        var validationResult = new FluentValidation.Results.ValidationResult([new ValidationFailure("Name", "Name is required")]);
        _mockValidator.ValidateAsync(experience).Returns(validationResult);

        // Act
        var result = await _service.CreateExperience(tenantId, experience);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
    }

    [Fact]
    public async Task CreateExperience_WebsiteNotFound_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        var experience = TestData.NewExperience;
        experience.SourceId = websiteId;
        _mockValidator.ValidateAsync(experience).Returns(new FluentValidation.Results.ValidationResult());
        _mockWebsiteService.GetWebsiteById(tenantId, websiteId)
            .Returns(OperationResult<Website>.NotFoundFailure("Website not found"));

        // Act
        var result = await _service.CreateExperience(tenantId, experience);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.ErrorMessage.Should().Contain("does not exist");
    }

    [Fact]
    public async Task CreateExperience_Success_GeneratesIdAndReturnsSuccess()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        var experience = TestData.NewExperience;
        experience.Id = Guid.Empty;
        experience.SourceId = websiteId;
        _mockValidator.ValidateAsync(experience).Returns(new FluentValidation.Results.ValidationResult());
        _mockWebsiteService.GetWebsiteById(tenantId, websiteId)
            .Returns(OperationResult<Website>.SuccessResult(new Website()));

        // Act
        var result = await _service.CreateExperience(tenantId, experience);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBe(Guid.Empty);
        experience.Id.Should().NotBe(Guid.Empty);
        await _mockExpRepo.Received(1).AddAsync(tenantId, experience);
    }
}

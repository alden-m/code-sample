using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValidation;
using BizName.Studio.App.Services;

namespace BizName.Studio.App.UnitTests.Services;

public class ExperienceServiceImplDeleteTests
{
    private readonly IPartitionedRepository<Experience> _mockExpRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IWebsiteService _mockWebsiteService = Substitute.For<IWebsiteService>();
    private readonly IValidator<Experience> _mockValidator = Substitute.For<IValidator<Experience>>();
    private readonly ExperienceServiceImpl _service;

    public ExperienceServiceImplDeleteTests()
    {
        _service = new ExperienceServiceImpl(_mockExpRepo, _mockWebsiteService, _mockValidator, NullLogger<ExperienceServiceImpl>.Instance);
    }

    [Fact]
    public async Task DeleteExperience_ExperienceNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();
        _mockExpRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns((Experience)null);

        // Act
        var result = await _service.DeleteExperience(tenantId, experienceId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteExperience_Success_DeletesExperience()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();
        var experience = TestData.NewExperience;
        _mockExpRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(experience);

        // Act
        var result = await _service.DeleteExperience(tenantId, experienceId);

        // Assert
        result.Success.Should().BeTrue();
        await _mockExpRepo.Received(1).DeleteAsync(tenantId, experience);
    }
}

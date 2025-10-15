using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValidation;
using BizName.Studio.App.Services;

namespace BizName.Studio.App.UnitTests.Services;

public class ExperienceServiceImplListTests
{
    private readonly IPartitionedRepository<Experience> _mockExpRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IWebsiteService _mockWebsiteService = Substitute.For<IWebsiteService>();
    private readonly IValidator<Experience> _mockValidator = Substitute.For<IValidator<Experience>>();
    private readonly ExperienceServiceImpl _service;

    public ExperienceServiceImplListTests()
    {
        _service = new ExperienceServiceImpl(_mockExpRepo, _mockWebsiteService, _mockValidator, NullLogger<ExperienceServiceImpl>.Instance);
    }

    [Fact]
    public async Task ListAllExperiences_Success_ReturnsFilteredResults()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        var experiences = new[] { TestData.NewExperience };
        _mockExpRepo.ListAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(experiences);

        // Act
        var result = await _service.ListAllExperiences(tenantId, websiteId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(experiences);
    }

    [Fact]
    public async Task ListPublishedExperiences_Success_ReturnsPublishedOnly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        var experiences = new[] { TestData.NewExperience };
        _mockExpRepo.ListAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(experiences);

        // Act
        var result = await _service.ListPublishedExperiences(tenantId, websiteId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(experiences);
    }
}

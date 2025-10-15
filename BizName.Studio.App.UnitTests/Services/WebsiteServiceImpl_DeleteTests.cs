using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValidation;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.UnitTests.Services;

public class WebsiteServiceImplDeleteTests
{
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IPartitionedRepository<Experience> _mockExperienceRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IValidator<Website> _mockValidator = Substitute.For<IValidator<Website>>();
    private readonly WebsiteServiceImpl _service;

    public WebsiteServiceImplDeleteTests()
    {
        _service = new WebsiteServiceImpl(_mockWebsiteRepo, _mockExperienceRepo, _mockValidator, NullLogger<WebsiteServiceImpl>.Instance);
    }

    [Fact]
    public async Task DeleteWebsite_WebsiteNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        _mockWebsiteRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns((Website)null);

        // Act
        var result = await _service.DeleteWebsite(tenantId, websiteId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteWebsite_HasExistingExperiences_ReturnsValidationError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        var website = TestData.NewWebsite;
        _mockWebsiteRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns(website);
        _mockExperienceRepo.ListAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(new List<Experience> { TestData.NewExperience });

        // Act
        var result = await _service.DeleteWebsite(tenantId, websiteId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.ErrorMessage.Should().Contain("Delete all existing experiences first");
    }

    [Fact]
    public async Task DeleteWebsite_NoExistingExperiences_ReturnsSuccess()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websiteId = Guid.NewGuid();
        var website = TestData.NewWebsite;
        _mockWebsiteRepo.GetAsync(tenantId, Arg.Any<Expression<Func<Website, bool>>>())
            .Returns(website);
        _mockExperienceRepo.ListAsync(tenantId, Arg.Any<Expression<Func<Experience, bool>>>())
            .Returns(new List<Experience>());

        // Act
        var result = await _service.DeleteWebsite(tenantId, websiteId);

        // Assert
        result.Success.Should().BeTrue();
        await _mockWebsiteRepo.Received(1).DeleteAsync(tenantId, website);
    }
}

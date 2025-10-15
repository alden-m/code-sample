using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValidation;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.UnitTests.Services;

public class WebsiteServiceImplListTests
{
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IPartitionedRepository<Experience> _mockExperienceRepo = Substitute.For<IPartitionedRepository<Experience>>();
    private readonly IValidator<Website> _mockValidator = Substitute.For<IValidator<Website>>();
    private readonly WebsiteServiceImpl _service;

    public WebsiteServiceImplListTests()
    {
        _service = new WebsiteServiceImpl(_mockWebsiteRepo, _mockExperienceRepo, _mockValidator, NullLogger<WebsiteServiceImpl>.Instance);
    }

    [Fact]
    public async Task ListWebsites_NoWebsites_ReturnsEmptyCollection()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _mockWebsiteRepo.ListAsync(tenantId)
            .Returns(new List<Website>());

        // Act
        var result = await _service.ListWebsites(tenantId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task ListWebsites_HasWebsites_ReturnsAllWebsites()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var websites = new List<Website>
        {
            TestData.NewWebsite,
            TestData.NewWebsite,
            TestData.NewWebsite
        };
        _mockWebsiteRepo.ListAsync(tenantId)
            .Returns(websites);

        // Act
        var result = await _service.ListWebsites(tenantId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.Should().BeEquivalentTo(websites);
    }
}

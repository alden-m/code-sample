using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesList_ValidationTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Get_MissingWebsiteIdParameter_Returns500InternalServerError()
    {
        // Arrange
        // No websiteId parameter

        // Act
        var response = await Client.GetAsync("/api/experiences");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Get_EmptyWebsiteIdParameter_Returns500InternalServerError()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={emptyGuid}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Get_InvalidGuidFormat_Returns400BadRequest()
    {
        // Arrange
        var invalidGuid = "invalid-guid-format";

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={invalidGuid}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_NonExistentWebsiteId_Returns200OK()
    {
        // Arrange
        var nonExistentWebsiteId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={nonExistentWebsiteId}");

        // Assert - Only test status code (API returns empty list for non-existent website)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

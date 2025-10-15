using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesDelete_ValidationTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Delete_NonExistentWebsite_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/websites/{nonExistentId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_InvalidGuidFormat_Returns404NotFound()
    {
        // Arrange
        var invalidGuid = "invalid-guid-format";

        // Act
        var response = await Client.DeleteAsync($"/api/websites/{invalidGuid}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

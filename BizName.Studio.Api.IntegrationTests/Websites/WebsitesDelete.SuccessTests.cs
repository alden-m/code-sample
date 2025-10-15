using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesDelete_SuccessTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Delete_ExistingWebsite_Returns204NoContent()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();

        // Act
        var response = await Client.DeleteAsync($"/api/websites/{websiteId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ExistingWebsite_RemovesFromDatabase()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();

        // Act
        await Client.DeleteAsync($"/api/websites/{websiteId}");
        var getResponse = await Client.GetAsync($"/api/websites/{websiteId}");

        // Assert - Only test persistence removal
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}

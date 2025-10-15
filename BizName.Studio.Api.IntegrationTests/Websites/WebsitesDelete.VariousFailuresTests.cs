using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesDelete_VariousFailuresTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Delete_AnonymousRequest_Returns401Unauthorized()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();

        // Act
        var response = await AnonymousClient.DeleteAsync($"/api/websites/{websiteId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_WebsiteWithExperiences_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        await CreateTestExperience(websiteId);

        // Act
        var response = await Client.DeleteAsync($"/api/websites/{websiteId}");

        // Assert - Only test status code (should fail due to existing experiences)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesUpdate_VariousFailuresTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task PutAnonymous_ValidWebsiteUpdate_Returns401Unauthorized()
    {
        // Arrange
        var website = WebsiteUpdateRequest.Valid();

        // Act
        var response = await AnonymousClient.PutAsJsonAsync("/api/websites", website);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Put_NonExistentWebsite_Returns404NotFound()
    {
        // Arrange
        var nonExistentWebsite = WebsiteUpdateRequest.Valid(); // This ID doesn't exist

        // Act
        var response = await Client.PutAsJsonAsync("/api/websites", nonExistentWebsite);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

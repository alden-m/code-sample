using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesCreate_VariousFailuresTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task PostAnonymous_ValidWebsite_Returns401Unauthorized()
    {
        // Arrange
        var website = WebsiteCreateRequest.Valid();

        // Act
        var response = await AnonymousClient.PostAsJsonAsync("/api/websites", website);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

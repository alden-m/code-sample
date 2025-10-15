using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesList_VariousFailuresTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Get_AnonymousRequest_Returns401Unauthorized()
    {
        // Arrange
        await CreateTestWebsite();

        // Act
        var response = await AnonymousClient.GetAsync("/api/websites");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}

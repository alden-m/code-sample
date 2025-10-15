using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesList_VariousFailuresTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Get_AnonymousRequest_Returns401Unauthorized()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        await CreateTestExperience(websiteId);

        // Act
        var response = await AnonymousClient.GetAsync($"/api/experiences?websiteId={websiteId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesDelete_VariousFailuresTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Delete_AnonymousRequest_Returns401Unauthorized()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experienceId = await CreateTestExperience(websiteId);

        // Act
        var response = await AnonymousClient.DeleteAsync($"/api/experiences/{experienceId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

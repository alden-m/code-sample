using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesDelete_SuccessTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Delete_ExistingExperience_Returns204NoContent()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experienceId = await CreateTestExperience(websiteId);

        // Act
        var response = await Client.DeleteAsync($"/api/experiences/{experienceId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ExistingExperience_RemovesFromDatabase()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experienceId = await CreateTestExperience(websiteId);

        // Act
        await Client.DeleteAsync($"/api/experiences/{experienceId}");
        var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");

        // Assert - Only test persistence removal
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

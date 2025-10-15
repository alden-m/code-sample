using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesDelete_ValidationTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Delete_NonExistentExperience_Returns404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/experiences/{nonExistentId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_InvalidGuidFormat_Returns404NotFound()
    {
        // Arrange
        var invalidGuid = "invalid-guid-format";

        // Act
        var response = await Client.DeleteAsync($"/api/experiences/{invalidGuid}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

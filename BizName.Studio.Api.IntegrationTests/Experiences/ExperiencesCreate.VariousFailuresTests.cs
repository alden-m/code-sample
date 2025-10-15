using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesCreate_VariousFailuresTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Post_Anonymous_Returns401Unauthorized()
    {
        // Arrange
        var experience = ExperienceCreateRequest.Valid();

        // Act
        var response = await AnonymousClient.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_NonExistentWebsiteId_Returns400BadRequest()
    {
        // Arrange
        var nonExistentWebsiteId = Guid.NewGuid();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(nonExistentWebsiteId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_NullBody_Returns400BadRequest()
    {
        // Arrange & Act
        var response = await Client.PostAsJsonAsync("/api/experiences", (ExperienceCreateRequest?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

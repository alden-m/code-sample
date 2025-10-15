using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesUpdate_VariousFailuresTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Put_NonExistentExperience_Returns404NotFound()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var nonExistentId = Guid.NewGuid();
        var updateRequest = ExperienceUpdateRequest.Valid(nonExistentId)
            .WithSourceId(websiteId);

        // Act
        var response = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("NotFound");
        errorResponse.StatusCode.Should().Be(404);
        errorResponse.Message.Should().Contain($"Experience with ID {nonExistentId} not found");
    }


    [Fact]
    public async Task Put_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var updateRequest = ExperienceUpdateRequest.Valid(Guid.NewGuid());

        // Act
        var response = await AnonymousClient.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Put_InvalidWebsiteId_Returns404NotFound()
    {
        // Arrange - Create experience with valid website
        var websiteId = await CreateTestWebsite();
        var initialExperience = ExperienceCreateRequest.Valid().WithSourceId(websiteId);
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with non-existent website ID
        var nonExistentWebsiteId = Guid.NewGuid();
        var updateRequest = ExperienceUpdateRequest.Valid(experienceId)
            .WithSourceId(nonExistentWebsiteId);

        // Act
        var response = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert - When updating to non-existent website, returns validation error
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.Message.Should().Contain($"Website with ID {nonExistentWebsiteId} does not exist");
    }
}

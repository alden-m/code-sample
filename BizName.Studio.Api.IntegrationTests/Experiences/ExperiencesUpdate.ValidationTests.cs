using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesUpdate_ValidationTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Put_MissingName_Returns400BadRequest()
    {
        // Arrange - Create initial experience
        var websiteId = await CreateTestWebsite();
        var initialExperience = ExperienceCreateRequest.Valid().WithSourceId(websiteId);
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with missing name
        var updateRequest = ExperienceUpdateRequest.Empty()
            .WithId(experienceId)
            .WithSourceId(websiteId)
            .WithName(""); // Invalid empty name

        // Act
        var response = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.ValidationErrors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Put_NullName_Returns400BadRequest()
    {
        // Arrange - Create initial experience
        var websiteId = await CreateTestWebsite();
        var initialExperience = ExperienceCreateRequest.Valid().WithSourceId(websiteId);
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with null name
        var updateRequest = ExperienceUpdateRequest.Empty()
            .WithId(experienceId)
            .WithSourceId(websiteId)
            .WithName(null); // Invalid null name

        // Act
        var response = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.ValidationErrors.Should().ContainKey("Name");
    }


    [Fact]
    public async Task Put_EmptyWebsiteId_Returns400BadRequest()
    {
        // Arrange - Create initial experience
        var websiteId = await CreateTestWebsite();
        var initialExperience = ExperienceCreateRequest.Valid().WithSourceId(websiteId);
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with empty website ID
        var updateRequest = ExperienceUpdateRequest.Valid(experienceId)
            .WithSourceId(Guid.Empty); // Empty GUID

        // Act
        var response = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.ValidationErrors.Should().ContainKey("WebsiteId");
    }

    [Fact]
    public async Task Put_NameTooLong_Returns400BadRequest()
    {
        // Arrange - Create initial experience
        var websiteId = await CreateTestWebsite();
        var initialExperience = ExperienceCreateRequest.Valid().WithSourceId(websiteId);
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with name exceeding max length
        var updateRequest = ExperienceUpdateRequest.Valid(experienceId)
            .WithSourceId(websiteId)
            .WithName(new string('a', 256)); // Assuming max length is 255

        // Act
        var response = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.ValidationErrors.Should().ContainKey("Name");
    }

}

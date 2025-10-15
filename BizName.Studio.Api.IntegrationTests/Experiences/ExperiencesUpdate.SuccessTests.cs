using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesUpdate_SuccessTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Put_ValidExperienceUpdate_Returns200Ok()
    {
        // Arrange - Create initial website and experience
        var websiteId = await CreateTestWebsite();
        
        var initialExperience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName("Original Experience Name")
            .WithIsPublished(false);
        
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Prepare update
        var updateRequest = ExperienceUpdateRequest.Valid(experienceId)
            .WithWebsiteId(websiteId)
            .WithName("Updated Experience Name")
            .WithIsPublished(false) // Cannot publish without actions/conditions
            .WithMetadata(new Dictionary<string, string> { { "key1", "value1" } });

        // Act
        var updateResponse = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert - Update successful
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedResult = await updateResponse.Content.ReadFromJsonAsync<ExperienceResponse>();
        updatedResult.Should().NotBeNull();
        updatedResult!.Id.Should().Be(experienceId);
        updatedResult.WebsiteId.Should().Be(websiteId);
        updatedResult.Name.Should().Be("Updated Experience Name");
        updatedResult.IsPublished.Should().BeFalse();
        updatedResult.Metadata.Should().ContainKey("key1").WhoseValue.Should().Be("value1");

        // Assert - Verify persistence
        var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedExperience = await getResponse.Content.ReadFromJsonAsync<ExperienceResponse>();
        retrievedExperience!.Name.Should().Be("Updated Experience Name");
        retrievedExperience.IsPublished.Should().BeFalse();
        retrievedExperience.Metadata.Should().ContainKey("key1");
    }

    [Fact]
    public async Task Put_UpdateOnlyName_Returns200OkWithOtherFieldsUnchanged()
    {
        // Arrange - Create initial website and experience with specific values
        var websiteId = await CreateTestWebsite();
        
        var initialExperience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName("Initial Name")
            .WithIsPublished(false) // Start unpublished
            .WithMetadata(new Dictionary<string, string> { { "initial", "data" } });
        
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Get the created experience to capture all values
        var getInitialResponse = await Client.GetAsync($"/api/experiences/{experienceId}");
        var initialData = await getInitialResponse.Content.ReadFromJsonAsync<ExperienceResponse>();

        // Arrange - Prepare update with only name changed
        var updateRequest = new ExperienceUpdateRequest
        {
            Id = experienceId,
            WebsiteId = websiteId,
            Name = "Updated Name Only",
            IsPublished = initialData!.IsPublished,
            Metadata = initialData.Metadata,
            Actions = initialData.Actions ?? new List<object>(),
            Conditions = initialData.Conditions ?? new List<object>()
        };

        // Act
        var updateResponse = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedResult = await updateResponse.Content.ReadFromJsonAsync<ExperienceResponse>();
        updatedResult!.Name.Should().Be("Updated Name Only");
        updatedResult.IsPublished.Should().Be(initialData.IsPublished);
        updatedResult.Metadata.Should().BeEquivalentTo(initialData.Metadata);
    }

}

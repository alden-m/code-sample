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
            .WithSourceId(websiteId)
            .WithName("Original Experience Name")
            .WithIsActive(false);
        
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", initialExperience);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Prepare update
        var updateRequest = ExperienceUpdateRequest.Valid(experienceId)
            .WithSourceId(websiteId)
            .WithName("Updated Experience Name")
            .WithIsActive(false) // Cannot publish without actions/conditions
            .WithConfiguration(new Dictionary<string, string> { { "key1", "value1" } });

        // Act
        var updateResponse = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert - Update successful
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedResult = await updateResponse.Content.ReadFromJsonAsync<ExperienceResponse>();
        updatedResult.Should().NotBeNull();
        updatedResult!.Id.Should().Be(experienceId);
        updatedResult.SourceId.Should().Be(websiteId);
        updatedResult.Name.Should().Be("Updated Experience Name");
        updatedResult.IsActive.Should().BeFalse();
        updatedResult.Configuration.Should().ContainKey("key1").WhoseValue.Should().Be("value1");

        // Assert - Verify persistence
        var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedExperience = await getResponse.Content.ReadFromJsonAsync<ExperienceResponse>();
        retrievedExperience!.Name.Should().Be("Updated Experience Name");
        retrievedExperience.IsActive.Should().BeFalse();
        retrievedExperience.Configuration.Should().ContainKey("key1");
    }

    [Fact]
    public async Task Put_UpdateOnlyName_Returns200OkWithOtherFieldsUnchanged()
    {
        // Arrange - Create initial website and experience with specific values
        var websiteId = await CreateTestWebsite();
        
        var initialExperience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId)
            .WithName("Initial Name")
            .WithIsActive(false) // Start unpublished
            .WithConfiguration(new Dictionary<string, string> { { "initial", "data" } });
        
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
            SourceId = websiteId,
            Name = "Updated Name Only",
            IsActive = initialData!.IsActive,
            Configuration = initialData.Configuration,
            Transformations = initialData.Transformations ?? new List<object>(),
            Rules = initialData.Rules ?? new List<object>()
        };

        // Act
        var updateResponse = await Client.PutAsJsonAsync("/api/experiences", updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedResult = await updateResponse.Content.ReadFromJsonAsync<ExperienceResponse>();
        updatedResult!.Name.Should().Be("Updated Name Only");
        updatedResult.IsActive.Should().Be(initialData.IsActive);
        updatedResult.Configuration.Should().BeEquivalentTo(initialData.Configuration);
    }

}

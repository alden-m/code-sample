using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesCreate_SuccessTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Post_ValidExperience_Returns201Created()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ValidExperience_ReturnsExperienceId()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert - Only test response format
        var experienceId = await response.Content.ReadFromJsonAsync<Guid>();
        experienceId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Post_ValidExperience_PersistsToDatabase()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId);
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", experience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Test the GET operation
        var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");

        // Assert - Only test persistence
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_ValidExperience_StoresCorrectPropertyValues()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId)
            .WithName("Test Experience")
            .WithIsActive(false);
        
        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", experience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");
        var retrievedExperience = await getResponse.Content.ReadFromJsonAsync<ExperienceResponse>();

        // Assert - Only test property values
        retrievedExperience!.Name.Should().Be("Test Experience");
        retrievedExperience.SourceId.Should().Be(websiteId);
        retrievedExperience.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Post_ValidExperience_DefaultsEmptyCollections()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId);
        
        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", experience);
        var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");
        var retrievedExperience = await getResponse.Content.ReadFromJsonAsync<ExperienceResponse>();

        // Assert - Only test default collections
        retrievedExperience!.Transformations.Should().BeEmpty();
        retrievedExperience.Rules.Should().BeEmpty();
    }

    [Fact]
    public async Task Post_ExperienceWithInsertHtmlAction_PersistsActionCorrectly()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithSourceId(websiteId)
            .WithInsertHtmlAction();
        
        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert - Test InsertHtmlAction persistence and serialization
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Only proceed if the create was successful
        if (createResponse.IsSuccessStatusCode)
        {
            var experienceId = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var getResponse = await Client.GetAsync($"/api/experiences/{experienceId}");
            var retrievedExperience = await getResponse.Content.ReadFromJsonAsync<ExperienceResponse>();

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            retrievedExperience!.Transformations.Should().HaveCount(1);
            retrievedExperience.Transformations.First().Should().NotBeNull();
        }
    }
}

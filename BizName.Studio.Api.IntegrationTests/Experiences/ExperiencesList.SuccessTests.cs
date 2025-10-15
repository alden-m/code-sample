using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesList_SuccessTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Get_ValidWebsiteId_Returns200OK()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        await CreateTestExperience(websiteId);

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={websiteId}");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ValidWebsiteId_ReturnsExperiencesList()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        await CreateTestExperience(websiteId);

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={websiteId}");
        var experiences = await response.Content.ReadFromJsonAsync<ExperienceResponse[]>();

        // Assert - Only test response format
        experiences.Should().NotBeNull();
        experiences.Should().BeOfType<ExperienceResponse[]>();
    }

    [Fact]
    public async Task Get_WebsiteWithNoExperiences_ReturnsEmptyList()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={websiteId}");
        var experiences = await response.Content.ReadFromJsonAsync<ExperienceResponse[]>();

        // Assert - Only test empty collection
        experiences.Should().NotBeNull();
        experiences.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_WebsiteWithMultipleExperiences_ReturnsAllExperiences()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        await CreateTestExperience(websiteId);
        await CreateTestExperience(websiteId);
        await CreateTestExperience(websiteId);

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={websiteId}");
        var experiences = await response.Content.ReadFromJsonAsync<ExperienceResponse[]>();

        // Assert - Only test count
        experiences.Should().NotBeNull();
        experiences.Should().HaveCount(3);
    }

    [Fact]
    public async Task Get_WebsiteWithExperiences_ReturnsCorrectPropertyValues()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experienceId1 = await CreateTestExperience(websiteId);
        var experienceId2 = await CreateTestExperience(websiteId);

        // Act
        var response = await Client.GetAsync($"/api/experiences?websiteId={websiteId}");
        var experiences = await response.Content.ReadFromJsonAsync<ExperienceResponse[]>();

        // Assert - Only test property values
        experiences.Should().NotBeNull();
        experiences.Should().Contain(e => e.Id == experienceId1);
        experiences.Should().Contain(e => e.Id == experienceId2);
        experiences.Should().OnlyContain(e => e.WebsiteId == websiteId);
    }

    [Fact]
    public async Task Get_WebsiteWithExperiences_FiltersCorrectlyByWebsite()
    {
        // Arrange
        var websiteId1 = await CreateTestWebsite();
        var experienceId1 = await CreateTestExperience(websiteId1);
        
        // Create second website and experience for a different test scenario
        var websiteId2 = await CreateTestWebsite();
        await CreateTestExperience(websiteId2); // Different website

        // Act - Only request experiences for first website
        var response = await Client.GetAsync($"/api/experiences?websiteId={websiteId1}");
        var experiences = await response.Content.ReadFromJsonAsync<ExperienceResponse[]>();

        // Assert - Only test filtering (should only return experiences for websiteId1)
        experiences.Should().NotBeNull();
        experiences.Should().HaveCount(1);
        experiences![0].Id.Should().Be(experienceId1);
        experiences[0].WebsiteId.Should().Be(websiteId1);
    }
}

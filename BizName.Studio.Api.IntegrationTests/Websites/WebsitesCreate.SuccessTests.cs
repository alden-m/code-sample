using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesCreate_SuccessTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Post_ValidWebsite_Returns201Created()
    {
        // Arrange
        var website = WebsiteCreateRequest.Valid();

        // Act
        var response = await Client.PostAsJsonAsync("/api/websites", website);

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_ValidWebsite_ReturnsWebsiteId()
    {
        // Arrange
        var website = WebsiteCreateRequest.Valid();

        // Act
        var response = await Client.PostAsJsonAsync("/api/websites", website);

        // Assert - Only test response format
        var websiteId = await response.Content.ReadFromJsonAsync<Guid>();
        websiteId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Post_ValidWebsite_PersistsToDatabase()
    {
        // Arrange
        var website = WebsiteCreateRequest.Valid();
        var createResponse = await Client.PostAsJsonAsync("/api/websites", website);
        var websiteId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Test the GET operation
        var getResponse = await Client.GetAsync($"/api/websites/{websiteId}");

        // Assert - Only test persistence
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_ValidWebsite_StoresCorrectPropertyValues()
    {
        // Arrange
        var uniqueName = $"Test Website {Guid.NewGuid():N}";
        var uniqueUrl = $"https://test-{Guid.NewGuid():N}.example.com";
        
        var website = WebsiteCreateRequest.Valid()
            .WithName(uniqueName)
            .WithEndpoint(uniqueUrl);
        
        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/websites", website);
        createResponse.EnsureSuccessStatusCode();
        var websiteId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        
        // Act - Get the created website
        var getResponse = await Client.GetAsync($"/api/websites/{websiteId}");
        var retrievedWebsite = await getResponse.Content.ReadFromJsonAsync<WebsiteResponse>();

        // Assert - Only test property values
        retrievedWebsite!.Name.Should().Be(uniqueName);
        retrievedWebsite.Endpoint.Should().Be(uniqueUrl);
    }
}

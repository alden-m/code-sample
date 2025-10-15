using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesUpdate_SuccessTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Put_ValidWebsiteUpdate_Returns200Ok()
    {
        // Arrange - Create initial website
        var initialWebsite = WebsiteCreateRequest.Valid();
        var createResponse = await Client.PostAsJsonAsync("/api/websites", initialWebsite);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var websiteId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Prepare update
        var updateRequest = WebsiteUpdateRequest.Valid(websiteId)
            .WithName("Updated Website Name")
            .WithUrl("https://updated-example.com");

        // Act
        var updateResponse = await Client.PutAsJsonAsync("/api/websites", updateRequest);

        // Assert - Update successful
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedResult = await updateResponse.Content.ReadFromJsonAsync<WebsiteResponse>();
        updatedResult.Should().NotBeNull();
        updatedResult!.Id.Should().Be(websiteId);
        updatedResult.Name.Should().Be("Updated Website Name");
        updatedResult.Url.Should().Be("https://updated-example.com");

        // Assert - Verify persistence
        var getResponse = await Client.GetAsync($"/api/websites/{websiteId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedWebsite = await getResponse.Content.ReadFromJsonAsync<WebsiteResponse>();
        retrievedWebsite!.Name.Should().Be("Updated Website Name");
        retrievedWebsite.Url.Should().Be("https://updated-example.com");
    }
}

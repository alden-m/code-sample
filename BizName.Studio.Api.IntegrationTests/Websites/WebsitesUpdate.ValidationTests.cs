using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesUpdate_ValidationTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Put_MissingName_Returns400BadRequest()
    {
        // Arrange - Create initial website
        var initialWebsite = WebsiteCreateRequest.Valid();
        var createResponse = await Client.PostAsJsonAsync("/api/websites", initialWebsite);
        var websiteId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with missing name
        var updateRequest = WebsiteUpdateRequest.Empty()
            .WithId(websiteId)
            .WithName("") // Invalid empty name
            .WithEndpoint("https://example.com");

        // Act
        var response = await Client.PutAsJsonAsync("/api/websites", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.ValidationErrors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Put_InvalidUrl_Returns400BadRequest()
    {
        // Arrange - Create initial website
        var initialWebsite = WebsiteCreateRequest.Valid();
        var createResponse = await Client.PostAsJsonAsync("/api/websites", initialWebsite);
        var websiteId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Arrange - Update with invalid URL
        var updateRequest = WebsiteUpdateRequest.Valid(websiteId)
            .WithEndpoint("not-a-valid-url"); // Invalid URL format

        // Act
        var response = await Client.PutAsJsonAsync("/api/websites", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Error.Should().Be("ValidationError");
        errorResponse.StatusCode.Should().Be(400);
        errorResponse.ValidationErrors.Should().ContainKey("Url");
    }
}

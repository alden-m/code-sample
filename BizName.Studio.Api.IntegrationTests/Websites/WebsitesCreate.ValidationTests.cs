using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesCreate_ValidationTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Post_MissingName_Returns400BadRequest()
    {
        // Arrange
        var website = WebsiteCreateRequest.Empty().WithUrl("https://example.com");

        // Act
        var response = await Client.PostAsJsonAsync("/api/websites", website);

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_MissingName_ReturnsValidationErrorForName()
    {
        // Arrange
        var website = WebsiteCreateRequest.Empty().WithUrl("https://example.com");

        // Act
        var response = await Client.PostAsJsonAsync("/api/websites", website);

        // Assert - Only test validation error key
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse!.ValidationErrors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Post_InvalidUrl_Returns400BadRequest()
    {
        // Arrange
        var website = WebsiteCreateRequest.Valid().WithUrl("not-a-valid-url");

        // Act
        var response = await Client.PostAsJsonAsync("/api/websites", website);

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_InvalidUrl_ReturnsValidationErrorForUrl()
    {
        // Arrange
        var website = WebsiteCreateRequest.Valid().WithUrl("not-a-valid-url");

        // Act
        var response = await Client.PostAsJsonAsync("/api/websites", website);

        // Assert - Only test validation error key
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse!.ValidationErrors.Should().ContainKey("Url");
    }
}

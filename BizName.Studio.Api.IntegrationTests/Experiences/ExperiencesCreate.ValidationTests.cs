using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Experiences;

public class ExperiencesCreate_ValidationTests(ApiTestFixture fixture) : ExperiencesTestBase(fixture)
{
    [Fact]
    public async Task Post_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName("");

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_EmptyName_ReturnsValidationErrorForName()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName("");

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert - Only test validation error key
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error!.ValidationErrors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Post_NullName_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName(null);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_EmptyWebsiteId_Returns400BadRequest()
    {
        // Arrange
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(Guid.Empty);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_NameTooLong_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var longName = new string('a', 101); // Exceeds 100 character limit
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName(longName);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_NameTooShort_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithName("a"); // Less than 2 characters

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_NullActions_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithActions(null);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_NullConditions_Returns400BadRequest()
    {
        // Arrange
        var websiteId = await CreateTestWebsite();
        var experience = ExperienceCreateRequest.Valid()
            .WithWebsiteId(websiteId)
            .WithConditions(null);

        // Act
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

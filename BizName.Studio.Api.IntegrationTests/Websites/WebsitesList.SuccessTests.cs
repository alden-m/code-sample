using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesList_SuccessTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Get_ValidRequest_Returns200OK()
    {
        // Arrange
        await CreateTestWebsite();

        // Act
        var response = await Client.GetAsync("/api/websites");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_WithoutData_Returns200OK()
    {
        // Arrange
        // No special setup needed for list operation

        // Act
        var response = await Client.GetAsync("/api/websites");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ValidRequest_ReturnsWebsitesList()
    {
        // Arrange
        await CreateTestWebsite();

        // Act
        var response = await Client.GetAsync("/api/websites");
        var websites = await response.Content.ReadFromJsonAsync<WebsiteResponse[]>();

        // Assert - Only test response format
        websites.Should().NotBeNull();
        websites.Should().BeOfType<WebsiteResponse[]>();
    }

    [Fact]
    public async Task Get_TenantWithNoWebsites_ReturnsEmptyList()
    {
        // Act
        var response = await Client.GetAsync("/api/websites");
        var websites = await response.Content.ReadFromJsonAsync<WebsiteResponse[]>();

        // Assert - Only test empty collection
        websites.Should().NotBeNull();
        websites.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_TenantWithMultipleWebsites_ReturnsAllWebsites()
    {
        // Arrange
        await CreateTestWebsite();
        await CreateTestWebsite();
        await CreateTestWebsite();

        // Act
        var response = await Client.GetAsync("/api/websites");
        var websites = await response.Content.ReadFromJsonAsync<WebsiteResponse[]>();

        // Assert - Only test count
        websites.Should().NotBeNull();
        websites.Should().HaveCount(3);
    }

    [Fact]
    public async Task Get_TenantWithWebsites_ReturnsCorrectPropertyValues()
    {
        // Arrange
        var websiteId1 = await CreateTestWebsite();
        var websiteId2 = await CreateTestWebsite();

        // Act
        var response = await Client.GetAsync("/api/websites");
        var websites = await response.Content.ReadFromJsonAsync<WebsiteResponse[]>();

        // Assert - Only test property values
        websites.Should().NotBeNull();
        websites.Should().Contain(w => w.Id == websiteId1);
        websites.Should().Contain(w => w.Id == websiteId2);
        websites.Should().OnlyContain(w => w.Id != Guid.Empty);
        websites.Should().OnlyContain(w => !string.IsNullOrEmpty(w.Name));
        websites.Should().OnlyContain(w => !string.IsNullOrEmpty(w.Url));
    }


}

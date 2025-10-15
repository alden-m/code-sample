using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public class WebsitesList_ValidationTests(ApiTestFixture fixture) : WebsitesTestBase(fixture)
{
    [Fact]
    public async Task Get_WithInvalidQueryParameters_Returns200OK()
    {
        // Arrange
        // List endpoints typically ignore invalid query parameters

        // Act
        var response = await Client.GetAsync("/api/websites?invalidParam=value");

        // Assert - Only test status code (should still work with invalid params)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_WithMalformedQueryString_Returns200OK()
    {
        // Arrange
        // List endpoints should handle malformed query strings gracefully

        // Act
        var response = await Client.GetAsync("/api/websites?=&malformed");

        // Assert - Only test status code
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

using System.Net;
using System.Text.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;

namespace BizName.Studio.Api.IntegrationTests.Features;

public record HealthCheckResponse(string Status, DateTime Timestamp);

public class HealthCheckTests(ApiTestFixture factory) : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _anonymousClient = factory.CreateClient();
    private readonly HttpClient _authenticatedClient = factory.CreateAuthenticatedClientWithUniqueTenant();
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task Get_AnonymousHealthCheckEndpoint_Returns200OK()
    {
        // Arrange
        const string endpoint = "/api/health/anonymous";

        // Act
        var response = await _anonymousClient.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var healthResponse = JsonSerializer.Deserialize<HealthCheckResponse>(content, JsonOptions);
        
        healthResponse.Should().NotBeNull();
        healthResponse!.Status.Should().Be("healthy");
        healthResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Get_HealthCheckEndpointWithAuthentication_Returns200OK()
    {
        // Arrange
        const string endpoint = "/api/health/anonymous";

        // Act
        var response = await _authenticatedClient.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var healthResponse = JsonSerializer.Deserialize<HealthCheckResponse>(content, JsonOptions);
        
        healthResponse.Should().NotBeNull();
        healthResponse!.Status.Should().Be("healthy");
        healthResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Get_SecureHealthCheckEndpointWithAuthentication_Returns200OK()
    {
        // Arrange
        const string endpoint = "/api/health/secure";

        // Act
        var response = await _authenticatedClient.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var healthResponse = JsonSerializer.Deserialize<HealthCheckResponse>(content, JsonOptions);
        
        healthResponse.Should().NotBeNull();
        healthResponse!.Status.Should().Be("healthy");
        healthResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Get_AnonymousSecureHealthCheckEndpoint_Returns401Unauthorized()
    {
        // Arrange
        const string endpoint = "/api/health/secure";

        // Act
        var response = await _anonymousClient.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

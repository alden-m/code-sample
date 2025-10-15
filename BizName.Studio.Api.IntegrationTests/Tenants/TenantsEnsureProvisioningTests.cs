using System.Net;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Contracts.Tenants;

namespace BizName.Studio.Api.IntegrationTests.Tenants;

public class TenantsEnsureProvisioningTests(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>, IAsyncLifetime
{
    private readonly HttpClient _client = fixture.CreateAuthenticatedClientWithUniqueTenant();

    [Fact]
    public async Task Post_ValidNewTenant_Returns204NoContent()
    {
        // Arrange
        var newTenant = TestData.NewValidTenant;

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/provision", newTenant);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/tenants/{newTenant.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedTenant = await getResponse.Content.ReadFromJsonAsync<Tenant>();
        retrievedTenant!.Id.Should().Be(newTenant.Id);
        retrievedTenant.Name.Should().Be(newTenant.Name);
    }

    [Fact]
    public async Task Post_ExistingTenant_Returns204NoContent()
    {
        // Arrange
        var existingTenant = TestData.NewValidTenant;

        await _client.PostAsJsonAsync("/api/tenants/provision", existingTenant);

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants/provision", existingTenant);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/tenants/{existingTenant.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedTenant = await getResponse.Content.ReadFromJsonAsync<Tenant>();
        retrievedTenant!.Id.Should().Be(existingTenant.Id);
        retrievedTenant.Name.Should().Be(existingTenant.Name);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

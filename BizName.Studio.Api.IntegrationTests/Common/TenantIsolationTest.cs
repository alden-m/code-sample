using System.Net.Http.Json;
using FluentAssertions;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Common;

public class TenantIsolationTest(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>
{
    [Fact]
    public async Task DifferentTenants_ShouldNotSeeEachOthersData()
    {
        // Arrange - Create clients for different tenants
        var tenant1Client = fixture.CreateAuthenticatedClientWithUniqueTenant();
        var tenant2Client = fixture.CreateAuthenticatedClientWithUniqueTenant();
        
        var website1 = WebsiteCreateRequest.Valid();
        var website2 = WebsiteCreateRequest.Valid();
        
        // Act - Create website in tenant 1
        var response1 = await tenant1Client.PostAsJsonAsync("/api/websites", website1);
        response1.EnsureSuccessStatusCode();
        
        // Create website in tenant 2  
        var response2 = await tenant2Client.PostAsJsonAsync("/api/websites", website2);
        response2.EnsureSuccessStatusCode();
        
        // Get websites for each tenant
        var tenant1Websites = await tenant1Client.GetFromJsonAsync<List<WebsiteResponse>>("/api/websites");
        var tenant2Websites = await tenant2Client.GetFromJsonAsync<List<WebsiteResponse>>("/api/websites");
        
        // Assert - Each tenant should only see their own website
        tenant1Websites.Should().HaveCount(1);
        tenant2Websites.Should().HaveCount(1);
        
        tenant1Websites![0].Name.Should().Be(website1.Name);
        tenant2Websites![0].Name.Should().Be(website2.Name);
        
        // Websites should have different IDs
        tenant1Websites[0].Id.Should().NotBe(tenant2Websites[0].Id);
        
        // Cleanup
        await tenant1Client.DeleteAsync($"/api/websites/{tenant1Websites[0].Id}");
        await tenant2Client.DeleteAsync($"/api/websites/{tenant2Websites[0].Id}");
    }
    
    [Fact]
    public async Task SameTenant_WithDifferentClients_ShouldSeeSharedData()
    {
        // Arrange - Create two clients for the same tenant
        var tenantId = TestData.NewUniqueTenantId;
        var client1 = CreateClientWithSpecificTenant(tenantId);
        var client2 = CreateClientWithSpecificTenant(tenantId);
        
        var website = WebsiteCreateRequest.Valid();
        
        // Act - Create website with client1
        var response = await client1.PostAsJsonAsync("/api/websites", website);
        response.EnsureSuccessStatusCode();
        var websiteId = await response.Content.ReadFromJsonAsync<Guid>();
        
        // Get websites with client2 (same tenant)
        var websites = await client2.GetFromJsonAsync<List<WebsiteResponse>>("/api/websites");
        
        // Assert - client2 should see the website created by client1
        websites.Should().HaveCount(1);
        websites![0].Id.Should().Be(websiteId);
        websites[0].Name.Should().Be(website.Name);
        
        // Cleanup
        await client1.DeleteAsync($"/api/websites/{websiteId}");
    }
    
    private HttpClient CreateClientWithSpecificTenant(Guid tenantId)
    {
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test", tenantId.ToString());
        return client;
    }
}

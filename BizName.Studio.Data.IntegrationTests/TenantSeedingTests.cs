using Microsoft.Extensions.DependencyInjection;
using BizName.Studio.App.Repositories;
using BizName.Studio.Contracts.Tenants;

namespace BizName.Studio.Data.IntegrationTests;

/// <summary>
/// Tests for seeding sample tenant data
/// </summary>
public class TenantSeedingTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task SeedSampleTenant_ShouldCreateTenantSuccessfully()
    {
        // Arrange
        var tenantRepository = fixture.ServiceProvider.GetRequiredService<IApplicationRepository<Tenant>>();
        
        var sampleTenant = Tenant.New(Guid.NewGuid(), "Sample Development Tenant");

        // Act
        await tenantRepository.AddAsync(sampleTenant);

        // Assert - Verify tenant was created
        var retrievedTenant = await tenantRepository.GetAsync(t => t.Id == sampleTenant.Id);
        
        Assert.NotNull(retrievedTenant);
        Assert.Equal(sampleTenant.Name, retrievedTenant.Name);
        Assert.Equal(sampleTenant.Id, retrievedTenant.Id);
    }

    [Fact]
    public async Task GetOrCreateDevelopmentTenant_ShouldReturnConsistentTenant()
    {
        // Arrange
        var tenantRepository = fixture.ServiceProvider.GetRequiredService<IApplicationRepository<Tenant>>();
        var developmentTenantName = "Development Tenant";

        // Act - Try to get existing development tenant
        var existingTenant = await tenantRepository.GetAsync(t => t.Name == developmentTenantName);
        
        Tenant developmentTenant;
        if (existingTenant == null)
        {
            // Create new development tenant
            developmentTenant = Tenant.New(Guid.NewGuid(), developmentTenantName);
            
            await tenantRepository.AddAsync(developmentTenant);
        }
        else
        {
            developmentTenant = existingTenant;
        }

        // Assert
        Assert.NotNull(developmentTenant);
        Assert.Equal(developmentTenantName, developmentTenant.Name);
        Assert.NotEqual(Guid.Empty, developmentTenant.Id);

        // Verify we can retrieve it again
        var verificationTenant = await tenantRepository.GetAsync(t => t.Id == developmentTenant.Id);
        Assert.NotNull(verificationTenant);
        Assert.Equal(developmentTenant.Name, verificationTenant.Name);
    }

    [Fact]
    public async Task ListAllTenants_ShouldReturnAllTenants()
    {
        // Arrange
        var tenantRepository = fixture.ServiceProvider.GetRequiredService<IApplicationRepository<Tenant>>();

        // Act
        var allTenants = await tenantRepository.ListAsync();

        // Assert
        Assert.NotNull(allTenants);
        // Should have at least the development tenant from previous tests
        Assert.True(allTenants.Any(), "Should have at least one tenant");
        
        // Verify each tenant has required properties
        foreach (var tenant in allTenants)
        {
            Assert.NotEqual(Guid.Empty, tenant.Id);
            Assert.False(string.IsNullOrWhiteSpace(tenant.Name));
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using BizName.Studio.Contracts.Experiences;
using BizName.Studio.App.Repositories;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.Data.IntegrationTests;

/// <summary>
/// Integration tests for repository operations across different entity types
/// </summary>
public class RepositoryIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task FullDataFlow_ShouldWorkCorrectly()
    {
        // Arrange
        var tenantRepository = fixture.ServiceProvider.GetRequiredService<IApplicationRepository<Tenant>>();
        var websiteRepository = fixture.ServiceProvider.GetRequiredService<IPartitionedRepository<Website>>();
        var experienceRepository = fixture.ServiceProvider.GetRequiredService<IPartitionedRepository<Experience>>();

        // Create a test tenant
        var testTenant = Tenant.New(Guid.NewGuid(), "Integration Test Tenant");

        // Create a test website
        var testWebsite = new Website
        {
            Id = Guid.NewGuid(),
            Name = "Test Website",
            Url = "https://test.example.com"
        };

        // Create a test experience
        var testExperience = new Experience
        {
            Id = Guid.NewGuid(),
            Name = "Test Experience",
            IsPublished = true,
            WebsiteId = testWebsite.Id
        };

        // Act & Assert
        
        // 1. Create tenant (application-level partition)
        await tenantRepository.AddAsync(testTenant);
        var retrievedTenant = await tenantRepository.GetAsync(t => t.Id == testTenant.Id);
        Assert.NotNull(retrievedTenant);
        Assert.Equal(testTenant.Name, retrievedTenant.Name);

        // 2. Create website (tenant-level partition)
        await websiteRepository.AddAsync(testTenant.Id, testWebsite);
        var retrievedWebsite = await websiteRepository.GetAsync(testTenant.Id, w => w.Id == testWebsite.Id);
        Assert.NotNull(retrievedWebsite);
        Assert.Equal(testWebsite.Name, retrievedWebsite.Name);
        Assert.Equal(testWebsite.Url, retrievedWebsite.Url);

        // 3. Create experience (tenant-level partition)
        await experienceRepository.AddAsync(testTenant.Id, testExperience);
        var retrievedExperience = await experienceRepository.GetAsync(testTenant.Id, e => e.Id == testExperience.Id);
        Assert.NotNull(retrievedExperience);
        Assert.Equal(testExperience.Name, retrievedExperience.Name);
        Assert.Equal(testExperience.IsPublished, retrievedExperience.IsPublished);

        // 4. List operations
        var tenantWebsites = await websiteRepository.ListAsync(testTenant.Id);
        Assert.Contains(tenantWebsites, w => w.Id == testWebsite.Id);

        var tenantExperiences = await experienceRepository.ListAsync(testTenant.Id);
        Assert.Contains(tenantExperiences, e => e.Id == testExperience.Id);

        // 5. Update operations
        testWebsite.Name = "Updated Test Website";
        await websiteRepository.UpdateAsync(testTenant.Id, testWebsite);
        var updatedWebsite = await websiteRepository.GetAsync(testTenant.Id, w => w.Id == testWebsite.Id);
        Assert.NotNull(updatedWebsite);
        Assert.Equal("Updated Test Website", updatedWebsite.Name);

        // 6. Delete operations
        await experienceRepository.DeleteAsync(testTenant.Id, testExperience);
        var deletedExperience = await experienceRepository.GetAsync(testTenant.Id, e => e.Id == testExperience.Id);
        Assert.Null(deletedExperience);

        await websiteRepository.DeleteAsync(testTenant.Id, testWebsite);
        var deletedWebsite = await websiteRepository.GetAsync(testTenant.Id, w => w.Id == testWebsite.Id);
        Assert.Null(deletedWebsite);

        await tenantRepository.DeleteAsync(testTenant);
        var deletedTenant = await tenantRepository.GetAsync(t => t.Id == testTenant.Id);
        Assert.Null(deletedTenant);
    }

    [Fact]
    public async Task TenantIsolation_ShouldWorkCorrectly()
    {
        // Arrange
        var tenantRepository = fixture.ServiceProvider.GetRequiredService<IApplicationRepository<Tenant>>();
        var websiteRepository = fixture.ServiceProvider.GetRequiredService<IPartitionedRepository<Website>>();

        var tenant1 = Tenant.New(Guid.NewGuid(), "Tenant 1");
        var tenant2 = Tenant.New(Guid.NewGuid(), "Tenant 2");

        var website1 = new Website { Id = Guid.NewGuid(), Name = "Website 1", Url = "https://site1.com" };
        var website2 = new Website { Id = Guid.NewGuid(), Name = "Website 2", Url = "https://site2.com" };

        // Act
        await tenantRepository.AddAsync(tenant1);
        await tenantRepository.AddAsync(tenant2);

        await websiteRepository.AddAsync(tenant1.Id, website1);
        await websiteRepository.AddAsync(tenant2.Id, website2);

        // Assert - Verify tenant isolation
        var tenant1Websites = await websiteRepository.ListAsync(tenant1.Id);
        var tenant2Websites = await websiteRepository.ListAsync(tenant2.Id);

        Assert.Single(tenant1Websites);
        Assert.Single(tenant2Websites);
        Assert.Equal(website1.Id, tenant1Websites.First().Id);
        Assert.Equal(website2.Id, tenant2Websites.First().Id);

        // Cleanup
        await websiteRepository.DeleteAsync(tenant1.Id, website1);
        await websiteRepository.DeleteAsync(tenant2.Id, website2);
        await tenantRepository.DeleteAsync(tenant1);
        await tenantRepository.DeleteAsync(tenant2);
    }
}

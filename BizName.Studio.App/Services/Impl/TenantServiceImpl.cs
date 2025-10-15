using FluentValidation;
using Microsoft.Extensions.Logging;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Extensions;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.Services.Impl;

internal class TenantServiceImpl(IApplicationRepository<Tenant> tenantRepository, IPartitionedRepository<Website> websiteRepository, IValidator<Tenant> tenantValidator, ILogger<TenantServiceImpl> logger) : ITenantService
{
    public async Task<OperationResult<Tenant>> GetById(Guid tenantId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
        logger.LogDebug("Getting tenant {TenantId}", tenantId);
        
        // Retrieve the tenant by tenantId
        var tenant = await tenantRepository.GetAsync(x => x.Id == tenantId);

        // Check if tenant is null
        if (tenant == null)
        {
            logger.LogWarning("Tenant {TenantId} not found", tenantId);
            return OperationResult<Tenant>.NotFoundFailure($"Tenant with ID {tenantId} not found");
        }
        
        logger.LogDebug("Found tenant {TenantId} with name {TenantName}", tenantId, tenant.Name);
        return OperationResult<Tenant>.SuccessResult(tenant);
    }

    public async Task<OperationResult> EnsureProvisioning(Tenant tenant)
    {
        // Argument validation
        if (tenant == null) throw new ArgumentNullException(nameof(tenant));
        if (tenant.Id == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenant));
            
        logger.LogDebug("Ensuring provisioning for tenant {TenantId} with name {TenantName}", tenant.Id, tenant.Name);
        
        // Check if the tenant already exists
        var existingTenant = await tenantRepository.GetAsync(x => x.Id == tenant.Id);

        // If tenant exists, return success
        if (existingTenant != null)
        {
            logger.LogDebug("Tenant {TenantId} already exists, provisioning complete", tenant.Id);
            return OperationResult.SuccessResult();
        }

        // Validate the new tenant before any database operations
        var validationResult = await tenantValidator.ValidateAsync(tenant);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Tenant validation failed for tenant {TenantId}: {ValidationErrors}", tenant.Id, string.Join(", ", validationResult.GetErrorMessages()));
            return validationResult.ToOperationResult();
        }

        // Add the new tenant (operation is immediate in Cosmos DB)
        await tenantRepository.AddAsync(tenant);
        
        logger.LogInformation("Provisioned new tenant {TenantId} with name {TenantName}", tenant.Id, tenant.Name);
        return OperationResult.SuccessResult();
    }

    public async Task<OperationResult> DeleteTenant(Guid tenantId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
        logger.LogDebug("Deleting tenant {TenantId}", tenantId);
        
        // Retrieve the tenant by tenantId
        var tenant = await tenantRepository.GetAsync(x => x.Id == tenantId);

        // Return failure if not found
        if (tenant == null)
        {
            logger.LogWarning("Tenant {TenantId} not found for deletion", tenantId);
            return OperationResult.NotFoundFailure($"Tenant with ID {tenantId} not found");
        }

        // Check if any websites exist for this tenant
        var websites = await websiteRepository.ListAsync(tenantId);
        if (websites.Any())
        {
            logger.LogWarning("Cannot delete tenant {TenantId}: {WebsiteCount} websites exist", tenantId, websites.Count());
            return OperationResult.ValidationFailure("Delete all existing websites first.");
        }

        // Remove the tenant (immediate operation in Cosmos DB)
        await tenantRepository.DeleteAsync(tenant);
        
        logger.LogInformation("Deleted tenant {TenantId} successfully", tenantId);
        return OperationResult.SuccessResult();
    }
}

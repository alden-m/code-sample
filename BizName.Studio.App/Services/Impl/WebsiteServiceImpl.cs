using FluentValidation;
using Microsoft.Extensions.Logging;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Experiences;
using BizName.Studio.Contracts.Extensions;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.Services.Impl;

internal class WebsiteServiceImpl(IPartitionedRepository<Website> websiteRepo, IPartitionedRepository<Experience> experienceRepo, IValidator<Website> websiteValidator, ILogger<WebsiteServiceImpl> logger) : IWebsiteService
{
    public async Task<OperationResult<IEnumerable<Website>>> ListWebsites(Guid tenantId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
        logger.LogDebug("Listing websites for tenant {TenantId}", tenantId);
        
        // Retrieve all websites for the given tenant
        var websites = await websiteRepo.ListAsync(tenantId);
        
        logger.LogDebug("Found {WebsiteCount} websites for tenant {TenantId}", websites.Count(), tenantId);
        return OperationResult<IEnumerable<Website>>.SuccessResult(websites);
    }

    public async Task<OperationResult<Guid>> CreateWebsite(Guid tenantId, Website website)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (website is null) throw new ArgumentNullException(nameof(website));
        
        logger.LogDebug("Creating website {WebsiteName} for tenant {TenantId}", website.Name, tenantId);
        
        // Validate website
        var validationResult = await websiteValidator.ValidateAsync(website);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Website validation failed for tenant {TenantId}: {ValidationErrors}", tenantId, string.Join(", ", validationResult.GetErrorMessages()));
            return validationResult.ToOperationResult<Guid>();
        }

        // Check for existing website with same name (conflict detection)
        var existingWebsites = await websiteRepo.ListAsync(tenantId, w => w.Name.ToLower() == website.Name.ToLower());
        if (existingWebsites.Any())
        {
            logger.LogWarning("Website name {WebsiteName} already exists for tenant {TenantId}", website.Name, tenantId);
            return OperationResult<Guid>.ValidationFailure($"A website with the name '{website.Name}' already exists");
        }

        // Ensure Id is set
        if (website.Id == Guid.Empty)
        {
            website.Id = Guid.NewGuid();
        }

        // Set timestamps for new website
        var now = DateTimeOffset.UtcNow;
        website.CreatedAt = now;
        website.UpdatedAt = now;

        // Save the website in the repository (immediate operation in Cosmos DB)
        await websiteRepo.AddAsync(tenantId, website);
        
        logger.LogInformation("Created website {WebsiteId} for tenant {TenantId} successfully", website.Id, tenantId);
        return OperationResult<Guid>.SuccessResult(website.Id);
    }

    public async Task<OperationResult<Website>> GetWebsiteById(Guid tenantId, Guid websiteId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (websiteId == Guid.Empty) throw new ArgumentException("WebsiteId cannot be empty", nameof(websiteId));
            
        logger.LogDebug("Getting website {WebsiteId} for tenant {TenantId}", websiteId, tenantId);
        // Retrieve the website by tenantId and websiteId
        var website = await websiteRepo.GetAsync(tenantId, x => x.Id == websiteId);

        // Return failure if not found
        if (website == null)
        {
            logger.LogWarning("Website {WebsiteId} not found for tenant {TenantId}", websiteId, tenantId);
            return OperationResult<Website>.NotFoundFailure($"Website with ID {websiteId} not found");
        }

        return OperationResult<Website>.SuccessResult(website);
    }

    public async Task<OperationResult<Website>> UpdateWebsite(Guid tenantId, Website website)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (website is null) throw new ArgumentNullException(nameof(website));
        if (website.Id == Guid.Empty) throw new ArgumentException("Website.Id cannot be empty", nameof(website));
            
        logger.LogDebug("Updating website {WebsiteId} for tenant {TenantId}", website.Id, tenantId);
        
        // Retrieve the website by tenantId and websiteId
        var existingWebsite = await websiteRepo.GetAsync(tenantId, x => x.Id == website.Id);

        // Return failure if not found
        if (existingWebsite == null)
        {
            logger.LogWarning("Website {WebsiteId} not found for update in tenant {TenantId}", website.Id, tenantId);
            return OperationResult<Website>.NotFoundFailure($"Website with ID {website.Id} not found");
        }

        // Validate the updated website using FluentValidation
        var validationResult = await websiteValidator.ValidateAsync(website);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Website update validation failed for {WebsiteId} in tenant {TenantId}: {ValidationErrors}", website.Id, tenantId, string.Join(", ", validationResult.GetErrorMessages()));
            return validationResult.ToOperationResult<Website>();
        }

        // Update the website's properties
        existingWebsite.Name = website.Name;
        existingWebsite.Description = website.Description;
        existingWebsite.Url = website.Url;
        // Preserve original creation date, update only the UpdatedAt timestamp
        existingWebsite.UpdatedAt = DateTimeOffset.UtcNow;

        // Save the updated website (immediate operation in Cosmos DB)
        await websiteRepo.UpdateAsync(tenantId, existingWebsite);
        
        logger.LogInformation("Updated website {WebsiteId} for tenant {TenantId} successfully", website.Id, tenantId);
        return OperationResult<Website>.SuccessResult(existingWebsite);
    }

    public async Task<OperationResult> DeleteWebsite(Guid tenantId, Guid websiteId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (websiteId == Guid.Empty) throw new ArgumentException("WebsiteId cannot be empty", nameof(websiteId));
            
        logger.LogDebug("Deleting website {WebsiteId} for tenant {TenantId}", websiteId, tenantId);
        
        // Retrieve the website by tenantId and websiteId
        var website = await websiteRepo.GetAsync(tenantId, x => x.Id == websiteId);

        // Return failure if not found
        if (website == null)
        {
            logger.LogWarning("Website {WebsiteId} not found for deletion in tenant {TenantId}", websiteId, tenantId);
            return OperationResult.NotFoundFailure($"Website with ID {websiteId} not found");
        }

        // Check if any experiences exist for this website
        var experiences = await experienceRepo.ListAsync(tenantId, x => x.WebsiteId == websiteId);
        if (experiences.Any())
        {
            logger.LogWarning("Cannot delete website {WebsiteId} for tenant {TenantId}: {ExperienceCount} experiences exist", websiteId, tenantId, experiences.Count());
            return OperationResult.ValidationFailure("Delete all existing experiences first.");
        }

        // Remove the website (immediate operation in Cosmos DB)
        await websiteRepo.DeleteAsync(tenantId, website);
        
        logger.LogInformation("Deleted website {WebsiteId} for tenant {TenantId} successfully", websiteId, tenantId);
        return OperationResult.SuccessResult();
    }

}

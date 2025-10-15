using FluentValidation;
using Microsoft.Extensions.Logging;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Experiences;
using BizName.Studio.Contracts.Extensions;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services;

namespace BizName.Studio.App.Services.Impl;

internal class ExperienceServiceImpl(IPartitionedRepository<Experience> expRepo, IWebsiteService websiteService, IValidator<Experience> experienceValidator, ILogger<ExperienceServiceImpl> logger) : IExperienceService
{
    public async Task<OperationResult<Guid>> CreateExperience(Guid tenantId, Experience experience)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (experience is null) throw new ArgumentNullException(nameof(experience));
        
        logger.LogDebug("Creating experience {ExperienceName} for tenant {TenantId}", experience.Name, tenantId);
        
        // Validate experience
        var validationResult = await experienceValidator.ValidateAsync(experience);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Experience validation failed for tenant {TenantId}: {ValidationErrors}", tenantId, string.Join(", ", validationResult.GetErrorMessages()));
            return validationResult.ToOperationResult<Guid>();
        }

        // Validate referential integrity - WebsiteId must exist
        logger.LogDebug("Validating website {WebsiteId} exists for tenant {TenantId}", experience.WebsiteId, tenantId);
        var websiteResult = await websiteService.GetWebsiteById(tenantId, experience.WebsiteId);
        if (!websiteResult.Success)
        {
            logger.LogWarning("Website {WebsiteId} not found for tenant {TenantId}", experience.WebsiteId, tenantId);
            return OperationResult<Guid>.ValidationFailure($"Website with ID {experience.WebsiteId} does not exist or you don't have access to it");
        }

        // Ensure Id is set
        if (experience.Id == Guid.Empty)
        {
            experience.Id = Guid.NewGuid();
        }

        await expRepo.AddAsync(tenantId, experience);
        
        logger.LogInformation("Created experience {ExperienceId} for tenant {TenantId} successfully", experience.Id, tenantId);
        return OperationResult<Guid>.SuccessResult(experience.Id);
    }

    public async Task<OperationResult<IEnumerable<Experience>>> ListAllExperiences(Guid tenantId, Guid websiteId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (websiteId == Guid.Empty) throw new ArgumentException("WebsiteId cannot be empty", nameof(websiteId));
            
        logger.LogDebug("Listing experiences for tenant {TenantId} and website {WebsiteId}", tenantId, websiteId);
        
        var experiences = await expRepo.ListAsync(tenantId, x => x.WebsiteId == websiteId);
        logger.LogDebug("Found {ExperienceCount} experiences for tenant {TenantId} and website {WebsiteId}", experiences.Count(), tenantId, websiteId);
        return OperationResult<IEnumerable<Experience>>.SuccessResult(experiences);
    }

    public async Task<OperationResult<Experience>> GetExperienceById(Guid tenantId, Guid experienceId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (experienceId == Guid.Empty) throw new ArgumentException("ExperienceId cannot be empty", nameof(experienceId));
            
        logger.LogDebug("Getting experience {ExperienceId} for tenant {TenantId}", experienceId, tenantId);
        var experience = await expRepo.GetAsync(tenantId, x => x.Id == experienceId);
        
        if (experience == null)
        {
            logger.LogWarning("Experience {ExperienceId} not found for tenant {TenantId}", experienceId, tenantId);
            return OperationResult<Experience>.NotFoundFailure($"Experience with ID {experienceId} not found");
        }

        return OperationResult<Experience>.SuccessResult(experience);
    }

    public async Task<OperationResult<IEnumerable<Experience>>> ListPublishedExperiences(Guid tenantId, Guid websiteId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (websiteId == Guid.Empty) throw new ArgumentException("WebsiteId cannot be empty", nameof(websiteId));
            
        logger.LogDebug("Listing published experiences for tenant {TenantId} and website {WebsiteId}", tenantId, websiteId);
        var experiences = await expRepo.ListAsync(tenantId, x => x.WebsiteId == websiteId && x.IsPublished);
        logger.LogDebug("Found {PublishedExperienceCount} published experiences for tenant {TenantId} and website {WebsiteId}", experiences.Count(), tenantId, websiteId);
        return OperationResult<IEnumerable<Experience>>.SuccessResult(experiences);
    }

    public async Task<OperationResult<Experience>> UpdateExperience(Guid tenantId, Guid experienceId, Experience updatedExperience)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (experienceId == Guid.Empty) throw new ArgumentException("ExperienceId cannot be empty", nameof(experienceId));
        if (updatedExperience is null) throw new ArgumentNullException(nameof(updatedExperience));
        
        logger.LogDebug("Updating experience {ExperienceId} for tenant {TenantId}", experienceId, tenantId);
        var existingExperience = await expRepo.GetAsync(tenantId, x => x.Id == experienceId);
        
        if (existingExperience == null)
        {
            logger.LogWarning("Experience {ExperienceId} not found for update in tenant {TenantId}", experienceId, tenantId);
            return OperationResult<Experience>.NotFoundFailure($"Experience with ID {experienceId} not found");
        }

        // Validate the updated experience using FluentValidation
        var validationResult = await experienceValidator.ValidateAsync(updatedExperience);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Experience update validation failed for {ExperienceId} in tenant {TenantId}: {ValidationErrors}", experienceId, tenantId, string.Join(", ", validationResult.GetErrorMessages()));
            return validationResult.ToOperationResult<Experience>();
        }

        // Validate referential integrity if WebsiteId is changing
        if (existingExperience.WebsiteId != updatedExperience.WebsiteId)
        {
            logger.LogDebug("Website ID changing for experience {ExperienceId}, validating new website {WebsiteId}", experienceId, updatedExperience.WebsiteId);
            var websiteResult = await websiteService.GetWebsiteById(tenantId, updatedExperience.WebsiteId);
            if (!websiteResult.Success)
            {
                logger.LogWarning("New website {WebsiteId} not found for experience {ExperienceId} update", updatedExperience.WebsiteId, experienceId);
                return OperationResult<Experience>.ValidationFailure($"Website with ID {updatedExperience.WebsiteId} does not exist or you don't have access to it");
            }
        }

        // Update all experience properties
        existingExperience.Name = updatedExperience.Name;
        existingExperience.IsPublished = updatedExperience.IsPublished;
        existingExperience.Conditions = updatedExperience.Conditions;
        existingExperience.Actions = updatedExperience.Actions;
        existingExperience.WebsiteId = updatedExperience.WebsiteId;
        existingExperience.Metadata = updatedExperience.Metadata;

        await expRepo.UpdateAsync(tenantId, existingExperience);
        
        logger.LogInformation("Updated experience {ExperienceId} for tenant {TenantId} successfully", experienceId, tenantId);
        return OperationResult<Experience>.SuccessResult(existingExperience);
    }

    public async Task<OperationResult> DeleteExperience(Guid tenantId, Guid experienceId)
    {
        // Argument validation
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
        if (experienceId == Guid.Empty) throw new ArgumentException("ExperienceId cannot be empty", nameof(experienceId));
            
        logger.LogDebug("Deleting experience {ExperienceId} for tenant {TenantId}", experienceId, tenantId);
        var experience = await expRepo.GetAsync(tenantId, x => x.Id == experienceId);
        
        if (experience == null)
        {
            logger.LogWarning("Experience {ExperienceId} not found for deletion in tenant {TenantId}", experienceId, tenantId);
            return OperationResult.NotFoundFailure("Experience not found.");
        }

        await expRepo.DeleteAsync(tenantId, experience);
        logger.LogInformation("Deleted experience {ExperienceId} for tenant {TenantId} successfully", experienceId, tenantId);
        return OperationResult.SuccessResult();
    }

}

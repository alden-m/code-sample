using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Experiences;

namespace BizName.Studio.App.Services;

public interface IExperienceService
{
    Task<OperationResult<Guid>> CreateExperience(Guid tenantId, Experience experience);
    Task<OperationResult<IEnumerable<Experience>>> ListAllExperiences(Guid tenantId, Guid websiteId);
    Task<OperationResult<IEnumerable<Experience>>> ListPublishedExperiences(Guid tenantId, Guid websiteId);
    Task<OperationResult<Experience>> GetExperienceById(Guid tenantId, Guid experienceId);
    Task<OperationResult<Experience>> UpdateExperience(Guid tenantId, Guid experienceId, Experience updatedExperience);
    Task<OperationResult> DeleteExperience(Guid tenantId, Guid experienceId);
}

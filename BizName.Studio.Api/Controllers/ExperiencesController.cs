using Microsoft.AspNetCore.Mvc;
using BizName.Studio.Api.Extensions;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Experiences;

namespace BizName.Studio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExperiencesController(IExperienceService experienceService, IRequestContext requestContext, ILogger<ExperiencesController> logger) : ControllerBase
{
    /// <summary>
    /// Creates a new experience with actions and conditions
    /// </summary>
    /// <param name="experience">The experience to create</param>
    /// <returns>The ID of the created experience</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateExperience([FromBody] Experience experience)
    {
        var tenantId = requestContext.TenantId;

        // Ensure server generates the ID - ignore any client-provided ID
        experience.Id = Guid.NewGuid();

        var result = await experienceService.CreateExperience(tenantId, experience);

        return result.Success
            ? CreatedAtAction(nameof(GetExperienceById), new { experienceId = result.Data }, result.Data)
            : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Retrieves a specific experience by its ID
    /// </summary>
    /// <param name="experienceId">The unique identifier of the experience to retrieve</param>
    /// <returns>The experience with all actions and conditions</returns>
    [HttpGet("{experienceId:guid}")]
    [ProducesResponseType(typeof(Experience), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExperienceById(Guid experienceId)
    {
        var tenantId = requestContext.TenantId;

        var result = await experienceService.GetExperienceById(tenantId, experienceId);

        return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Lists all experiences for a specific website
    /// </summary>
    /// <param name="websiteId">The ID of the website to list experiences for</param>
    /// <returns>A list of experiences with their actions and conditions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Experience>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllExperiences([FromQuery] Guid websiteId)
    {
        var tenantId = requestContext.TenantId;

        var result = await experienceService.ListAllExperiences(tenantId, websiteId);

        return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Updates an existing experience with new actions and conditions
    /// </summary>
    /// <param name="experience">The experience to update with modified properties</param>
    /// <returns>The updated experience</returns>
    [HttpPut]
    [ProducesResponseType(typeof(Experience), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateExperience([FromBody] Experience experience)
    {
        var tenantId = requestContext.TenantId;

        var result = await experienceService.UpdateExperience(tenantId, experience.Id, experience);

        return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Deletes an existing experience
    /// </summary>
    /// <param name="experienceId">The unique identifier of the experience to delete</param>
    /// <returns>No content on successful deletion</returns>
    [HttpDelete("{experienceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteExperience(Guid experienceId)
    {
        var tenantId = requestContext.TenantId;

        var result = await experienceService.DeleteExperience(tenantId, experienceId);

        return result.Success ? NoContent() : result.ToErrorResponse(HttpContext);
    }
}

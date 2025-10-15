using Microsoft.AspNetCore.Mvc;
using BizName.Studio.Api.Extensions;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebsitesController(IWebsiteService websiteService, IRequestContext requestContext) : ControllerBase
{
    /// <summary>
    /// Creates a new website
    /// </summary>
    /// <param name="website">The website to create</param>
    /// <returns>The ID of the created website</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWebsite([FromBody] Website website)
    {
        var tenantId = requestContext.TenantId;

        // Ensure server generates the ID - ignore any client-provided ID
        website.Id = Guid.NewGuid();

        var result = await websiteService.CreateWebsite(tenantId, website);

        return result.Success 
            ? CreatedAtAction(nameof(GetWebsiteById), new { websiteId = result.Data }, result.Data)
            : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Retrieves a specific website by its ID
    /// </summary>
    /// <param name="websiteId">The unique identifier of the website to retrieve</param>
    /// <returns>The website details</returns>
    [HttpGet("{websiteId:guid}")]
    [ProducesResponseType(typeof(Website), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWebsiteById(Guid websiteId)
    {
        var tenantId = requestContext.TenantId;

        var result = await websiteService.GetWebsiteById(tenantId, websiteId);

        return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Lists all websites for the current tenant
    /// </summary>
    /// <returns>A list of websites</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Website>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWebsites()
    {
        var tenantId = requestContext.TenantId;

        var result = await websiteService.ListWebsites(tenantId);

        return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Updates an existing website
    /// </summary>
    /// <param name="website">The website to update with modified properties</param>
    /// <returns>The updated website</returns>
    [HttpPut]
    [ProducesResponseType(typeof(Website), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateWebsite([FromBody] Website website)
    {
        var tenantId = requestContext.TenantId;

        var result = await websiteService.UpdateWebsite(tenantId, website);

        return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
    }

    /// <summary>
    /// Deletes an existing website
    /// </summary>
    /// <param name="websiteId">The unique identifier of the website to delete</param>
    /// <returns>No content on successful deletion</returns>
    [HttpDelete("{websiteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteWebsite(Guid websiteId)
    {
        var tenantId = requestContext.TenantId;

        var result = await websiteService.DeleteWebsite(tenantId, websiteId);

        return result.Success ? NoContent() : result.ToErrorResponse(HttpContext);
    }
}

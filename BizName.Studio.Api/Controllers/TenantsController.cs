using Microsoft.AspNetCore.Mvc;
using BizName.Studio.Api.Extensions;
using BizName.Studio.App.Services;
using BizName.Studio.Contracts.Tenants;

namespace BizName.Studio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        /// <summary>
        /// Ensures tenant provisioning is complete with provided tenant data
        /// </summary>
        /// <param name="tenant">The tenant data to provision</param>
        /// <returns>No content on successful provisioning</returns>
        [HttpPost("provision")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> EnsureProvisioning([FromBody] Tenant tenant)
        {
            var result = await _tenantService.EnsureProvisioning(tenant);

            return result.Success ? NoContent() : result.ToErrorResponse(HttpContext);
        }

        /// <summary>
        /// Retrieves a specific tenant by its ID
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant to retrieve</param>
        /// <returns>The tenant details</returns>
        [HttpGet("{tenantId:guid}")]
        [ProducesResponseType(typeof(Tenant), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid tenantId)
        {
            var result = await _tenantService.GetById(tenantId);

            return result.Success ? Ok(result.Data) : result.ToErrorResponse(HttpContext);
        }

        /// <summary>
        /// Deletes a specific tenant by its ID
        /// </summary>
        /// <param name="tenantId">The unique identifier of the tenant to delete</param>
        /// <returns>No content on successful deletion</returns>
        [HttpDelete("{tenantId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTenant(Guid tenantId)
        {
            var result = await _tenantService.DeleteTenant(tenantId);

            return result.Success ? NoContent() : result.ToErrorResponse(HttpContext);
        }
    }
}

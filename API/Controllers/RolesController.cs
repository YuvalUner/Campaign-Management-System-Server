using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : Controller
{
    private readonly ILogger<RolesController> _logger;
    private readonly IRolesService _rolesService;
    
    public RolesController(ILogger<RolesController> logger, IRolesService rolesService)
    {
        _logger = logger;
        _rolesService = rolesService;
    }

    [HttpGet("get-roles/{campaignGuid:guid}")]
    public async Task<IActionResult> GetRoles(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRolesList,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized();
            }

            var roles = await _rolesService.GetRolesInCampaign(campaignGuid);
            return Ok(roles);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting roles in campaign");
            return StatusCode(500);
        }
    }
    
    [HttpPost("add-role/{campaignGuid:guid}")]
    public async Task<IActionResult> AddRole(Guid campaignGuid, [FromBody] Role role)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRolesList,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }

            var res = await _rolesService.AddRoleToCampaign(campaignGuid, role.RoleName, role.RoleDescription);
            return Ok(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding role to campaign");
            return StatusCode(500);
        }
    }
}
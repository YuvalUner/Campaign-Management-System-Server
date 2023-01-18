using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : Controller
{
    private readonly IPermissionsService _permissionService;
    private readonly ILogger<PermissionsController> _logger;
    private readonly IUsersService _usersService;

    public PermissionsController(IPermissionsService permissionService, ILogger<PermissionsController> logger,
        IUsersService usersService)
    {
        _permissionService = permissionService;
        _logger = logger;
        _usersService = usersService;
    }

    [HttpPost("add/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddPermission(Guid campaignGuid, string userEmail,
        [FromBody] Permission permission)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            // TODO: Check if user has permission to edit permissions
            // TODO: Check if user can edit this permission
            User? user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            await _permissionService.AddPermission(permission, user.UserId, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding permission");
            return StatusCode(500);
        }
    }

    [HttpGet("get/{campaignGuid:guid}")]
    public async Task<IActionResult> GetPermissions(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            if (userId == null)
            {
                return Unauthorized();
            }

            var permissions = await _permissionService.GetPermissions(userId, campaignGuid);
            return Ok(permissions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting permissions");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("delete/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> DeletePermission(Guid campaignGuid, string userEmail,
        [FromBody] Permission permission)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            // TODO: Check if user has permission to edit permissions
            // TODO: Check if user can edit this permission
            User? user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            await _permissionService.RemovePermission(permission, user.UserId, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing permission");
            return StatusCode(500);
        }
    }
}
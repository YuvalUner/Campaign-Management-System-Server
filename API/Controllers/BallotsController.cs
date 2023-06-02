using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BallotsController : Controller
{
    private readonly IBallotsService _ballotsService;
    private readonly ILogger<BallotsController> _logger;
    
    public BallotsController(IBallotsService ballotsService, ILogger<BallotsController> logger)
    {
        _ballotsService = ballotsService;
        _logger = logger;
    }

    /// <summary>
    /// API endpoint for adding a new custom ballot for a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="ballot"></param>
    /// <returns>Conflict in case of already existing ballot with the same id, Unauthorized if user does not
    /// have permission, BadRequest in case of wrong campaign Guid or bad ballot id, Ok otherwise.</returns>
    [HttpPost("add-ballot/{campaignGuid:guid}")]
    public async Task<IActionResult> AddCustomBallot(Guid campaignGuid, [FromBody] Ballot ballot)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.BallotManagement,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            if (ballot.InnerCityBallotId is null or < 0)
            {
                return BadRequest(FormatErrorMessage(IllegalBallotId, CustomStatusCode.IllegalValue));
            }

            var result = await _ballotsService.AddCustomBallot(ballot, campaignGuid);
            return result switch
            {
                CustomStatusCode.DuplicateKey => Conflict(FormatErrorMessage(DuplicateBallotId,
                    CustomStatusCode.DuplicateKey)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to add ballot");
            return StatusCode(500, "Error while trying to add ballot");
        }
    }
    
    /// <summary>
    /// API endpoint for updating a custom ballot for a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="ballot"></param>
    /// <returns>Unauthorized if user does not have permission, BadRequest in case of wrong campaign Guid or bad ballot id,
    /// Ok otherwise.</returns>
    [HttpPut("update-ballot/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateCustomBallot(Guid campaignGuid, [FromBody] Ballot ballot)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.BallotManagement,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            if (ballot.InnerCityBallotId is null or < 0)
            {
                return BadRequest(FormatErrorMessage(IllegalBallotId, CustomStatusCode.IllegalValue));
            }

            var result = await _ballotsService.UpdateCustomBallot(ballot, campaignGuid);
            return result switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to update ballot");
            return StatusCode(500, "Error while trying to update ballot");
        }
    }
    
    /// <summary>
    /// API endpoint for deleting a custom ballot for a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="innerCityBallotId"></param>
    /// <returns>Unauthorized if user does not have permission, BadRequest in case of wrong campaign Guid or bad ballot id,
    /// Ok otherwise.</returns>
    [HttpDelete("delete-ballot/{campaignGuid:guid}/{innerCityBallotId:decimal}")]
    public async Task<IActionResult> DeleteCustomBallot(Guid campaignGuid, decimal innerCityBallotId)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.BallotManagement,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            if (innerCityBallotId < 0)
            {
                return BadRequest(FormatErrorMessage(IllegalBallotId, CustomStatusCode.IllegalValue));
            }

            var result = await _ballotsService.DeleteCustomBallot(innerCityBallotId, campaignGuid);
            return result switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to delete ballot");
            return StatusCode(500, "Error while trying to delete ballot");
        }
    }
    
    /// <summary>
    /// API endpoint for getting all ballots for a campaign, both default and custom ones.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    [HttpGet("get-all-ballots/{campaignGuid:guid}")]
    public async Task<IActionResult> GetAllBallotsForCampaign(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.BallotManagement,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var result = await _ballotsService.GetAllBallotsForCampaign(campaignGuid);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get all ballots");
            return StatusCode(500, "Error while trying to get all ballots");
        }
    }
}
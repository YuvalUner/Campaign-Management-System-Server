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
public class PartiesController : Controller
{
    private readonly IPartiesService _partiesService;
    private readonly ILogger<PartiesController> _logger;
    
    public PartiesController(IPartiesService partiesService, ILogger<PartiesController> logger)
    {
        _partiesService = partiesService;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new party to the database for the given campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="party"></param>
    /// <returns>Unauthorized for permission errors, BadRequest for bad party name, NotFound for bad campaignGuid,
    /// Ok otherwise.</returns>
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddParty(Guid campaignGuid, [FromBody] Party party)
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

            if (string.IsNullOrWhiteSpace(party.PartyName))
            {
                return BadRequest(FormatErrorMessage(PartyNameRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            var result = await _partiesService.AddParty(party, campaignGuid);
            return result switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding party.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    /// <summary>
    /// Updates the given party for the given campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="party"></param>
    /// <returns>Unauthorized for permission errors, BadRequest for bad party name or party id,
    /// NotFound for bad campaign guid, Ok otherwise.</returns>
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateParty(Guid campaignGuid, [FromBody] Party party)
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

            if (string.IsNullOrWhiteSpace(party.PartyName))
            {
                return BadRequest(FormatErrorMessage(PartyNameRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            if (party.PartyId is null or < 0)
            {
                return BadRequest(FormatErrorMessage(IllegalPartyId, CustomStatusCode.IllegalValue));
            }
            
            var result = await _partiesService.UpdateParty(party, campaignGuid);
            return result switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating party.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    /// <summary>
    /// Deletes the party with the given party id for the given campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="partyId"></param>
    /// <returns>Unauthorized for permission errors, BadRequest for bad party id, Ok otherwise.</returns>
    [HttpDelete("delete/{campaignGuid:guid}/{partyId:int}")]
    public async Task<IActionResult> DeleteParty(Guid campaignGuid, int partyId)
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
            
            if (partyId < 0)
            {
                return BadRequest(FormatErrorMessage(IllegalPartyId, CustomStatusCode.IllegalValue));
            }
            
            var result = await _partiesService.DeleteParty(partyId, campaignGuid);
            return result switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting party.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    /// <summary>
    /// Gets all parties for the given campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns>Unauthorized in case of permission error, Ok with a list of parties otherwise.</returns>
    [HttpGet("get/{campaignGuid:guid}")]
    public async Task<IActionResult> GetParties(Guid campaignGuid)
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
            
            var result = await _partiesService.GetParties(campaignGuid);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting parties.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
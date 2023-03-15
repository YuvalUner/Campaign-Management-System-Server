using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomStatusCode = DAL.DbAccess.CustomStatusCode;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for getting data from the voters ledger, as well as modifying some dynamic data in it.
/// Provides a web API and service policy for <see cref="IVotersLedgerService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VotersLedgerController : Controller
{
    private readonly IVotersLedgerService _votersLedgerService;
    private readonly ILogger<VotersLedgerController> _logger;
    private readonly ICampaignsService _campaignsService;

    public VotersLedgerController(IVotersLedgerService votersLedgerService, ILogger<VotersLedgerController> logger,
        ICampaignsService campaignsService)
    {
        _votersLedgerService = votersLedgerService;
        _logger = logger;
        _campaignsService = campaignsService;
    }

    /// <summary>
    /// Filters the voters ledger by the given parameters.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign making the request.</param>
    /// <param name="filter">An instance of <see cref="VotersLedgerFilter"/> containing all the filters required as
    /// non null values.</param>
    /// <returns>Unauthorized if the user does not have permission to view the voters ledger,
    /// BadRequest if the campaign type is national and no city name, first name, last name or id number was given,
    /// Ok with a list of <see cref="VotersLedgerFilterRecord"/> otherwise.</returns>
    [HttpPost("/filter/{campaignGuid:guid}")]
    public async Task<IActionResult> FilterVotersLedger(Guid campaignGuid, [FromBody] VotersLedgerFilter filter)
    {
        try
        {
            // Check that the user has access to the campaign
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.VotersLedger,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Verify that the campaign type is municipal or that that the campaign type is national and 
            // either a city was given or a first and last name or id number was given.
            CampaignType campaignType = await _campaignsService.GetCampaignType(campaignGuid);
            if (!campaignType.IsMunicipal && String.IsNullOrEmpty(filter.CityName)
                                          && String.IsNullOrEmpty(filter.FirstName)
                                          && String.IsNullOrEmpty(filter.LastName)
                                          && filter.IdNum == null)
            {
                return BadRequest(FormatErrorMessage(CityNameRequired, CustomStatusCode.CityNameRequired));
            }

            if (campaignType.IsMunicipal)
            {
                filter.CityName = campaignType.CityName;
            }

            filter.CampaignGuid = campaignGuid;
            return Ok(await _votersLedgerService.GetFilteredVotersLedgerResults(filter));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while filtering voters ledger");
            return StatusCode(500, "Error while filtering voters ledger");
        }
    }

    /// <summary>
    /// Updates the support status of a voter in the voters ledger.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to change support status for.</param>
    /// <param name="updateParams">An instance of <see cref="UpdateSupportStatusParams"/> with all the required info.</param>
    /// <returns>Unauthorized if the user does not have permission to edit the voters ledger,
    /// BadRequest if the voter could not be found in that city, Ok otherwise.</returns>
    [HttpPut("/updateSupportStatus/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateSupportStatus(Guid campaignGuid,
        [FromBody] UpdateSupportStatusParams updateParams)
    {
        try
        {
            // Check that the user has access to the campaign
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.VotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var res = await _votersLedgerService.UpdateVoterSupportStatus(updateParams, campaignGuid);
            return res switch
            {
                CustomStatusCode.CityNotFound => BadRequest(FormatErrorMessage(CityNotFound,
                    CustomStatusCode.CityNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating support status");
            return StatusCode(500, "Error while updating support status");
        }
    }
}
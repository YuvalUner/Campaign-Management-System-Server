﻿using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    
    [HttpGet("/filter/{campaignGuid:guid}")]
    public async Task<IActionResult> FilterVotersLedger(Guid campaignGuid, [FromQuery] VotersLedgerFilter filter)
    {
        try
        {
            // Check that the user has access to the campaign
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            // Make sure user has view permissions for the voters ledger
            var requiredPermission = new Permission()
            {
                PermissionType = PermissionTypes.View,
                PermissionTarget = PermissionTargets.VotersLedger
            };
            if (!PermissionUtils.HasPermission(HttpContext, requiredPermission))
            {
                return Unauthorized();
            }

            // Verify that the campaign type is municipal or that that the campaign type is national and 
            // either a city was given or a first and last name or id number was given.
            CampaignType campaignType = await _campaignsService.GetCampaignType(campaignGuid);
            if (!campaignType.IsMunicipal && String.IsNullOrEmpty(filter.CityName)
                                          && String.IsNullOrEmpty(filter.FirstName)
                                          && String.IsNullOrEmpty(filter.LastName)
                                          && filter.IdNum == null)
            {
                return BadRequest("City name is required for non-municipal" +
                                  " campaigns when the search is not by first name, last name or id");
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
}
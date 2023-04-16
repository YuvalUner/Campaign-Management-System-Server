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
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class CustomVotersLedgerController : Controller
{
    private readonly ICustomVotersLedgerService _customVotersLedgerService;
    private readonly ILogger<CustomVotersLedgerController> _logger;

    public CustomVotersLedgerController(ICustomVotersLedgerService customVotersLedgerService,
        ILogger<CustomVotersLedgerController> logger)
    {
        _customVotersLedgerService = customVotersLedgerService;
        _logger = logger;
    }

    [HttpPost("create/{campaignGuid:guid}")]
    public async Task<IActionResult> CreateCustomLedger(Guid campaignGuid,
        [FromBody] CustomVotersLedger customVotersLedger)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            customVotersLedger.CampaignGuid = campaignGuid;
            
            var (statusCode, guid) = await _customVotersLedgerService.CreateCustomVotersLedger(customVotersLedger);

            return statusCode switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok(new { LedgerGuid = guid })
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to create custom ledger");
            return StatusCode(500, "Error while trying to create custom ledger");
        }
    }
    
    [HttpGet("get-for-campaign/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCustomLedgersForCampaign(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var customVotersLedgers = await _customVotersLedgerService.GetCustomVotersLedgersByCampaignGuid(campaignGuid);

            return Ok(customVotersLedgers);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get custom ledgers for campaign");
            return StatusCode(500, "Error while trying to get custom ledgers for campaign");
        }
    }
    
    [HttpDelete("delete/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> DeleteCustomLedger(Guid campaignGuid, Guid ledgerGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var statusCode = await _customVotersLedgerService.DeleteCustomVotersLedger(ledgerGuid);

            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to delete custom ledger");
            return StatusCode(500, "Error while trying to delete custom ledger");
        }
    }
    
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateCustomLedger(Guid campaignGuid,
        [FromBody] CustomVotersLedger customVotersLedger)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var statusCode = await _customVotersLedgerService.UpdateCustomVotersLedger(customVotersLedger);

            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to update custom ledger");
            return StatusCode(500, "Error while trying to update custom ledger");
        }
    }
    
}
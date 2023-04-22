﻿using API.Utils;
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

    #region Custom Voters Ledgers
    
    
    /// <summary>
    /// Creates a new custom voters ledger for the given campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to add a ledger to.</param>
    /// <param name="customVotersLedger">An instance of <see cref="CustomVotersLedger"/> with the name of the new ledger.</param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Gets all of a campaign's custom voters ledgers.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
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
    
    
    /// <summary>
    /// Deletes an existing custom voters ledger for the given campaign.<br/>
    /// This will also delete all data associated with the ledger.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to delete for.</param>
    /// <param name="ledgerGuid">Guid of the ledger to delete.</param>
    /// <returns></returns>
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

            var statusCode = await _customVotersLedgerService.DeleteCustomVotersLedger(ledgerGuid, campaignGuid);

            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                CustomStatusCode.BoundaryViolation => BadRequest(FormatErrorMessage(AuthorizationError, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to delete custom ledger");
            return StatusCode(500, "Error while trying to delete custom ledger");
        }
    }
    
    /// <summary>
    /// Updates the name of an existing custom voters ledger for the given campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the ledger belongs to.</param>
    /// <param name="customVotersLedger">An instance of <see cref="CustomVotersLedger"/> with the name and
    /// ledgerGuid properties not null.</param>
    /// <returns></returns>
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

            var statusCode = await _customVotersLedgerService.UpdateCustomVotersLedger(customVotersLedger, campaignGuid);

            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                CustomStatusCode.BoundaryViolation => BadRequest(FormatErrorMessage(AuthorizationError, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to update custom ledger");
            return StatusCode(500, "Error while trying to update custom ledger");
        }
    }
    
    #endregion
    
    #region Custom Voters Ledger Rows

    [HttpPost("add-row/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> AddRowToCustomLedger(Guid campaignGuid, Guid ledgerGuid,
        CustomVotersLedgerContent content)
    {
        try
        {
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
            
            if (content.Identifier == null)
            {
                return BadRequest(FormatErrorMessage(IdentifierMissing, CustomStatusCode.ValueCanNotBeNull));
            }
            
            var statusCode = await _customVotersLedgerService.AddCustomVotersLedgerRow(content, ledgerGuid);
            
            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.DuplicateKey => BadRequest(FormatErrorMessage(RowAlreadyExists, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to add row to custom ledger");
            return StatusCode(500, "Error while trying to add row to custom ledger");
        }
    }
    
    [HttpDelete("delete-row/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> DeleteRowFromCustomLedger(Guid campaignGuid, Guid ledgerGuid, [FromQuery] int? rowId)
    {
        try
        {
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
            
            if (rowId == null)
            {
                return BadRequest(FormatErrorMessage(IdentifierMissing, CustomStatusCode.ValueCanNotBeNull));
            }

            var statusCode = await _customVotersLedgerService.DeleteCustomVotersLedgerRow(ledgerGuid, rowId.Value);
            
            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.LedgerRowNotFound => NotFound(FormatErrorMessage(LedgerRowNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to delete row from custom ledger");
            return StatusCode(500, "Error while trying to delete row from custom ledger");
        }
    }
    
    [HttpPut("update-row/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> UpdateRowInCustomLedger(Guid campaignGuid, Guid ledgerGuid,
        CustomVotersLedgerContent content)
    {
        try
        {
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
            
            if (content.Identifier == null)
            {
                return BadRequest(FormatErrorMessage(IdentifierMissing, CustomStatusCode.ValueCanNotBeNull));
            }
            
            var statusCode = await _customVotersLedgerService.UpdateCustomVotersLedgerRow(content, ledgerGuid);
            
            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.LedgerRowNotFound => NotFound(FormatErrorMessage(LedgerRowNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to update row in custom ledger");
            return StatusCode(500, "Error while trying to update row in custom ledger");
        }
    }

    [HttpGet("filter/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> FilterCustomLedger(Guid campaignGuid, Guid ledgerGuid,
        CustomLedgerFilterParams filter)
    {
        try
        {
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
            
            var res = await _customVotersLedgerService.FilterCustomVotersLedger(ledgerGuid, filter);

            return Ok(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to filter custom ledger");
            return StatusCode(500, "Error while trying to filter custom ledger");
        }
    }
    
    #endregion
    
}
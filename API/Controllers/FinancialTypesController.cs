using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

// Disable warning CS8509: The switch expression does not handle all possible values of its input type (it is not exhaustive).
// This is due to switches not including the Ok case - this is intentional, as the code with the switch in it is only ever 
// reached if the result is not Ok, so the Ok case is not needed.
#pragma warning disable CS8509

namespace API.Controllers;

/// <summary>
/// Controller for financial types.<br/>
/// Generally, exposes a web API for <see cref="IFinancialTypesService"/>, allowing the client to perform CRUD
/// operations on financial types.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Consumes("application/json")]
public class FinancialTypesController : Controller
{
    private readonly IFinancialTypesService _financialTypesService;
    private readonly ILogger<FinancialTypesController> _logger;

    public FinancialTypesController(IFinancialTypesService financialTypesService,
        ILogger<FinancialTypesController> logger)
    {
        _financialTypesService = financialTypesService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the list of financial types for a campaign.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <returns>Unauthorized if the user does not have permission to view the campaign's financial info,
    /// Ok with the list of <see cref="FinancialType"/> otherwise.</returns>
    [HttpGet("get-for-campaign/{campaignGuid:guid}")]
    public async Task<IActionResult> GetFinancialTypesForCampaign(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Financial,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var financialTypes = await _financialTypesService.GetFinancialTypes(campaignGuid);

            return Ok(financialTypes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting financial types for campaign");
            return StatusCode(500, "Error while getting financial types for campaign");
        }
    }

    /// <summary>
    /// Creates a new financial type for a campaign.
    /// </summary>
    /// <param name="campaignGuid">The campaign's Guid</param>
    /// <param name="financialType">A <see cref="FinancialType"/> object with at-least the name filled in.</param>
    /// <returns>Unauthorized if the user does not have permission to edit the campaign's financial info,
    /// BadRequest if there is an issue with the info in the financialType parameter, NotFound if the campaign
    /// could not be found, Ok with the Guid of the new financial type on success.</returns>
    [HttpPost("create/{campaignGuid:guid}")]
    public async Task<IActionResult> CreateFinancialType(Guid campaignGuid, FinancialType financialType)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Financial,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Validate the user input
            var validationFailureCode = financialType.VerifyLegalValues();
            if (validationFailureCode != FinancialType.ValidationFailureCodes.Ok)
            {
                return BadRequest(FormatErrorMessage(validationFailureCode switch
                {
                    FinancialType.ValidationFailureCodes.TitleTooLong => InvalidFinancialTypeName,
                    FinancialType.ValidationFailureCodes.DescriptionTooLong => InvalidFinancialTypeDescription,
                }, CustomStatusCode.IllegalValue));
            }

            var (statusCode, typeGuid) = await _financialTypesService.CreateFinancialType(financialType);

            return statusCode switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                CustomStatusCode.TooManyEntries => StatusCode(500,
                    FormatErrorMessage(TooManyFinancialTypes, statusCode)),
                _ => Ok(new { typeGuid })
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating financial type");
            return StatusCode(500, "Error while creating financial type");
        }
    }

    /// <summary>
    /// Updates an existing financial type.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <param name="financialType">A <see cref="FinancialType"/> object with at-least TypeGuid filled in, and any field
    /// that the user wishes to change also filled in.</param>
    /// <returns>Unauthorized if the user does not have permission to edit the campaign's financial info,
    /// BadRequest if there is an issue with the info in the financialType parameter, NotFound if the financial type
    /// could not be found, Ok otherwise.</returns>
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateFinancialType(Guid campaignGuid, FinancialType financialType)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Financial,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var validationFailureCode = financialType.VerifyLegalValues(isCreation: false);
            if (validationFailureCode != FinancialType.ValidationFailureCodes.Ok)
            {
                return BadRequest(FormatErrorMessage(validationFailureCode switch
                {
                    FinancialType.ValidationFailureCodes.TitleTooLong => InvalidFinancialTypeName,
                    FinancialType.ValidationFailureCodes.DescriptionTooLong => InvalidFinancialTypeDescription,
                }, CustomStatusCode.IllegalValue));
            }

            var statusCode = await _financialTypesService.UpdateFinancialType(financialType);

            return statusCode switch
            {
                CustomStatusCode.FinancialTypeNotFound => NotFound(
                    FormatErrorMessage(FinancialTypeNotFound, statusCode)),
                CustomStatusCode.SqlIllegalValue => BadRequest(FormatErrorMessage(CanNotModifyThisType, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating financial type");
            return StatusCode(500, "Error while updating financial type");
        }
    }

    /// <summary>
    /// Deletes an existing financial type from a campaign.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <param name="typeGuid">The Guid of the financial type.</param>
    /// <returns>Unauthorized if the user does not have permission to edit the campaign's financial info,
    /// BadRequest if the Guid is of a built in type, NotFound if the financial type
    /// could not be found, Ok otherwise.</returns>
    [HttpDelete("delete/{campaignGuid:guid}/{typeGuid:guid}")]
    public async Task<IActionResult> DeleteFinancialType(Guid campaignGuid, Guid typeGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Financial,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var statusCode = await _financialTypesService.DeleteFinancialType(typeGuid);

            return statusCode switch
            {
                CustomStatusCode.FinancialTypeNotFound => NotFound(
                    FormatErrorMessage(FinancialTypeNotFound, statusCode)),
                CustomStatusCode.SqlIllegalValue => BadRequest(FormatErrorMessage(CanNotModifyThisType, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting financial type");
            return StatusCode(500, "Error while deleting financial type");
        }
    }
}
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
[Produces("application/json")]
[Consumes("application/json")]
public class FinancialTypesController : Controller
{
    private readonly IFinancialTypesService _financialTypesService;
    private readonly ILogger<FinancialTypesController> _logger;
    private readonly int _maxFinancialTypeTitleLength = 100;
    private readonly int _maxFinancialTypeDescriptionLength = 300;

    public FinancialTypesController(IFinancialTypesService financialTypesService,
        ILogger<FinancialTypesController> logger)
    {
        _financialTypesService = financialTypesService;
        _logger = logger;
    }

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
            if (string.IsNullOrWhiteSpace(financialType.TypeName) ||
                financialType.TypeName.Length > _maxFinancialTypeTitleLength)
            {
                return BadRequest(FormatErrorMessage(InvalidFinancialTypeName, CustomStatusCode.IllegalValue));
            }

            if (financialType.TypeDescription != null &&
                financialType.TypeDescription.Length > _maxFinancialTypeDescriptionLength)
            {
                return BadRequest(FormatErrorMessage(InvalidFinancialTypeDescription, CustomStatusCode.IllegalValue));
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
            
            // Validate the user input
            if (string.IsNullOrWhiteSpace(financialType.TypeName) ||
                financialType.TypeName.Length > _maxFinancialTypeTitleLength)
            {
                return BadRequest(FormatErrorMessage(InvalidFinancialTypeName, CustomStatusCode.IllegalValue));
            }

            if (financialType.TypeDescription != null &&
                financialType.TypeDescription.Length > _maxFinancialTypeDescriptionLength)
            {
                return BadRequest(FormatErrorMessage(InvalidFinancialTypeDescription, CustomStatusCode.IllegalValue));
            }

            var statusCode = await _financialTypesService.UpdateFinancialType(financialType);

            return statusCode switch
            {
                CustomStatusCode.FinancialTypeNotFound => NotFound(
                    FormatErrorMessage(FinancialTypeNotFound, statusCode)),
                CustomStatusCode.SqlIllegalValue => NotFound(FormatErrorMessage(CanNotModifyThisType, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating financial type");
            return StatusCode(500, "Error while updating financial type");
        }
    }

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
                CustomStatusCode.SqlIllegalValue => NotFound(FormatErrorMessage(CanNotModifyThisType, statusCode)),
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
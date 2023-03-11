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
public class FinancialDataController : Controller
{
    private readonly IFinancialDataService _financialDataService;
    private readonly ILogger<FinancialDataController> _logger;

    public FinancialDataController(IFinancialDataService financialDataService, ILogger<FinancialDataController> logger)
    {
        _financialDataService = financialDataService;
        _logger = logger;
    }

    [HttpGet("get-for-campaign/{campaignGuid:guid}")]
    public async Task<IActionResult> GetFinancialDataForCampaign(Guid campaignGuid, [FromQuery] Guid? financialTypeGuid)
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

            var financialData =
                await _financialDataService.GetFinancialDataForCampaign(campaignGuid, financialTypeGuid);

            return Ok(financialData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting financial data for campaign");
            return StatusCode(500, "Error while getting financial data for campaign");
        }
    }

    [HttpPost("get-summary-for-campaign/{campaignGuid:guid}")]
    public async Task<IActionResult> GetFinancialDataSummaryForCampaign(Guid campaignGuid)
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

            var financialDataSummary = await _financialDataService.GetFinancialSummary(campaignGuid);

            return Ok(financialDataSummary);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting financial data summary for campaign");
            return StatusCode(500, "Error while getting financial data summary for campaign");
        }
    }
    
    [HttpPost("create/{campaignGuid:guid}")]
    public async Task<IActionResult> CreateFinancialData(Guid campaignGuid, FinancialDataEntry financialData)
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

            var validationStatus = financialData.VerifyLegalValues();
            if (validationStatus != FinancialDataEntry.ValidationFailureCodes.Ok)
            {
                return validationStatus switch
                {
                    FinancialDataEntry.ValidationFailureCodes.TitleTooLong => BadRequest(FormatErrorMessage(
                        FinancialDataTitleTooLong, CustomStatusCode.IllegalValue)),
                    FinancialDataEntry.ValidationFailureCodes.DescriptionTooLong => BadRequest(FormatErrorMessage(
                        FinancialDataDescriptionTooLong, CustomStatusCode.IllegalValue)),
                    FinancialDataEntry.ValidationFailureCodes.AmountTooLow => BadRequest(FormatErrorMessage(IllegalAmount,
                        CustomStatusCode.IllegalValue)),
                };
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            financialData.CampaignGuid = campaignGuid;
            financialData.CreatorUserId = userId;

            var (statusCode, newGuid) = await _financialDataService.AddFinancialDataEntry(financialData);

            return statusCode switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                CustomStatusCode.FinancialTypeNotFound => NotFound(FormatErrorMessage(FinancialTypeNotFound,
                    CustomStatusCode.FinancialTypeNotFound)),
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                _ => Ok(new { newGuid })
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating financial data");
            return StatusCode(500, "Error while creating financial data");
        }
    }
    
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateFinancialData(Guid campaignGuid, FinancialDataEntry financialData)
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

            var validationStatus = financialData.VerifyLegalValues(isCreation: false);
            if (validationStatus != FinancialDataEntry.ValidationFailureCodes.Ok)
            {
                return validationStatus switch
                {
                    FinancialDataEntry.ValidationFailureCodes.TitleTooLong => BadRequest(FormatErrorMessage(
                        FinancialDataTitleTooLong, CustomStatusCode.IllegalValue)),
                    FinancialDataEntry.ValidationFailureCodes.DescriptionTooLong => BadRequest(FormatErrorMessage(
                        FinancialDataDescriptionTooLong, CustomStatusCode.IllegalValue)),
                    FinancialDataEntry.ValidationFailureCodes.AmountTooLow => BadRequest(FormatErrorMessage(IllegalAmount,
                        CustomStatusCode.IllegalValue)),
                };
            }

            var statusCode = await _financialDataService.UpdateFinancialDataEntry(financialData);

            return statusCode switch
            {
                CustomStatusCode.FinancialTypeNotFound => NotFound(FormatErrorMessage(FinancialTypeNotFound,
                    CustomStatusCode.FinancialTypeNotFound)),
                CustomStatusCode.FinancialDataNotFound => NotFound(FormatErrorMessage(FinancialDataNotFound,
                    CustomStatusCode.FinancialDataNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating financial data");
            return StatusCode(500, "Error while updating financial data");
        }
    }
    
    [HttpDelete("delete/{campaignGuid:guid}/{financialDataGuid:guid}")]
    public async Task<IActionResult> DeleteFinancialData(Guid campaignGuid, Guid financialDataGuid)
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

            var statusCode = await _financialDataService.DeleteFinancialDataEntry(financialDataGuid);

            return statusCode switch
            {
                CustomStatusCode.FinancialDataNotFound => NotFound(FormatErrorMessage(FinancialDataNotFound,
                    CustomStatusCode.FinancialDataNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting financial data");
            return StatusCode(500, "Error while deleting financial data");
        }
    }
}
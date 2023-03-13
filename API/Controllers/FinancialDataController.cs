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
/// A controller for handling financial data related requests.<br/>
/// Generally, exposes a web API for <see cref="IFinancialDataService"/> methods,
/// allowing the client to perform CRUD operations on financial data.
/// </summary>
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

    /// <summary>
    /// Gets financial data for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="financialTypeGuid">The Guid of a specific financial type. If provided, only entries regarding that
    /// financial type will be retrieved.</param>
    /// <returns>Unauthorized if the user does not have permission to view the campaign's financial data, Ok with a list
    /// of <see cref="FinancialDataEntryWithTypeAndCreator"/> sorted by date on success.</returns>
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

    /// <summary>
    /// Gets a campaign's financial summary.<br/>
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>Unauthorized if the user does not have permission to view the campaign's finances.
    /// Otherwise, an object where the total incomes, total expenses and total balance is listed, followed by a list
    /// that specifies each of these for each financial type.</returns>
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

            // Converting to a list to avoid multiple enumerations.
            var financialSummaryEntries = financialDataSummary.ToList();

            var incomes = financialSummaryEntries.Where(x => !x.IsExpense).ToList();
            var expenses = financialSummaryEntries.Where(x => x.IsExpense).ToList();

            var balances = new List<FinancialSummaryBalance>();

            // For every income, find the matching expense and calculate the balance.
            foreach (var income in incomes)
            {
                var matchingExpense = expenses.FirstOrDefault(x => x.TypeGuid == income.TypeGuid);
                if (matchingExpense != null)
                {
                    balances.Add(new FinancialSummaryBalance()
                    {
                        TypeGuid = income.TypeGuid,
                        TypeName = income.TypeName,
                        Balance = income.TotalAmount - matchingExpense.TotalAmount,
                        IncomeTotal = income.TotalAmount,
                        ExpenseTotal = matchingExpense.TotalAmount
                    });
                }
                else
                {
                    balances.Add(new FinancialSummaryBalance()
                    {
                        TypeGuid = income.TypeGuid,
                        TypeName = income.TypeName,
                        Balance = income.TotalAmount,
                        IncomeTotal = income.TotalAmount,
                        ExpenseTotal = 0
                    });
                }
            }

            // Get the totals of all expenses, incomes and the balance.
            var totalExpenses = expenses.Sum(x => x.TotalAmount);
            var totalIncome = incomes.Sum(x => x.TotalAmount);
            var totalBalance = totalIncome - totalExpenses;

            return Ok(new
            {
                totalExpenses,
                totalIncome,
                totalBalance,
                balances
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting financial data summary for campaign");
            return StatusCode(500, "Error while getting financial data summary for campaign");
        }
    }

    /// <summary>
    /// Creates a new financial data entry for the specified campaign.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <param name="financialData">A populated <see cref="FinancialDataEntry"/> object.</param>
    /// <returns>Unauthorized if the user may not edit financial data in the campaign, BadRequest in case some of the
    /// data in the financialData parameter is not ok (title too long, amount below 0, etc), NotFound if the campaign,
    /// financial type or creator could not be found Ok with the Guid of the new entry on success.</returns>
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
                    FinancialDataEntry.ValidationFailureCodes.AmountTooLow => BadRequest(FormatErrorMessage(
                        IllegalAmount,
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

    /// <summary>
    /// Updates a financial data entry.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <param name="financialData">A populated <see cref="FinancialDataEntry"/> object with its Guid, and any field that
    /// should be updated filled in.</param>
    /// <returns>Unauthorized if the user may not edit financial data in the campaign, BadRequest in case some of the
    /// data in the financialData parameter is not ok (title too long, amount below 0, etc), NotFound if the financial data 
    /// or financial type could not be found Ok with the Guid of the new entry on success.</returns>
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

            // Verify that the financial data entry is valid.
            var validationStatus = financialData.VerifyLegalValues(isCreation: false);
            if (validationStatus != FinancialDataEntry.ValidationFailureCodes.Ok)
            {
                return validationStatus switch
                {
                    FinancialDataEntry.ValidationFailureCodes.TitleTooLong => BadRequest(FormatErrorMessage(
                        FinancialDataTitleTooLong, CustomStatusCode.IllegalValue)),
                    FinancialDataEntry.ValidationFailureCodes.DescriptionTooLong => BadRequest(FormatErrorMessage(
                        FinancialDataDescriptionTooLong, CustomStatusCode.IllegalValue)),
                    FinancialDataEntry.ValidationFailureCodes.AmountTooLow => BadRequest(FormatErrorMessage(
                        IllegalAmount,
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

    /// <summary>
    /// Deletes a financial data entry from a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="financialDataGuid">Guid of the financial data to delete.</param>
    /// <returns>Unauthorized if the user may not edit the campaign's financial data, NotFound if the data does not exist,
    /// Ok on success.</returns>
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
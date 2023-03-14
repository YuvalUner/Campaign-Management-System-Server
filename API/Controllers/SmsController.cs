using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SmsController: Controller
{
    private readonly ISmsMessageService _smsMessageService;
    private readonly ILogger<SmsController> _logger;
    
    public SmsController(ISmsMessageService smsMessageService, ILogger<SmsController> logger)
    {
        _smsMessageService = smsMessageService;
        _logger = logger;
    }
    
    [HttpPost("/send/{campaignGuid:guid}")]
    public async Task<IActionResult> SendSmsMessages(Guid campaignGuid, [FromBody] SmsSendingParams smsSendingParams)
    {
        try
        {
            // Check that the user has access to the campaign
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Sms,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            if (smsSendingParams.PhoneNumbers == null || smsSendingParams.PhoneNumbers.Count == 0)
            {
                return BadRequest(FormatErrorMessage(PhoneNumbersRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            if (string.IsNullOrEmpty(smsSendingParams.MessageContents))
            {
                return BadRequest(FormatErrorMessage(MessageContentRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            smsSendingParams.CampaignGuid = campaignGuid;
            smsSendingParams.SenderId = HttpContext.Session.GetInt32(Constants.UserId);
            // Remove any empty strings
            smsSendingParams.PhoneNumbers.RemoveAll(String.IsNullOrEmpty);
            // Remove any possible duplicates and prevent some fun duplicate key errors and double sending
            smsSendingParams.PhoneNumbers = smsSendingParams.PhoneNumbers.Distinct().ToList();

            // Add the message to the database
            (Guid newMessageGuid, int newMessageId) = await _smsMessageService.AddSmsMessage(smsSendingParams, CountryCodes.Israel);
            
            // Send the message to all the phone numbers
            // This is not awaited on purpose - it can take a long time, and we don't want not have reason to wait for it.
            _smsMessageService.SendSmsMessageToMultiplePhones(smsSendingParams, newMessageId, CountryCodes.Israel);
            
            return Ok(new {newMessageGuid});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending SMS messages");
            return StatusCode(500, "Error sending SMS messages");
        }
    }
    
    [HttpGet("/logs/{campaignGuid:guid}")]
    public async Task<IActionResult> GetSmsLogs(Guid campaignGuid)
    {
        try
        {
            // Check that the user has access to the campaign
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Sms,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var smsLogs = await _smsMessageService.GetBaseSmsLogs(campaignGuid);
            return Ok(smsLogs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting SMS logs");
            return StatusCode(500, "Error getting SMS logs");
        }
    }
    
    [HttpGet("/logs/details/{campaignGuid:guid}/{messageGuid:guid}")]
    public async Task<IActionResult> GetSmsDetailsLog(Guid campaignGuid, Guid messageGuid)
    {
        try
        {
            // Check that the user has access to the campaign
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Sms,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var smsDetailsLogs = await _smsMessageService.GetSmsDetailsLog(messageGuid);
            return Ok(smsDetailsLogs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting SMS details logs");
            return StatusCode(500, "Error getting SMS details logs");
        }
    }
}
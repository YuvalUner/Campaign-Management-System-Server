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
public class VerificationCodeController: Controller
{
    private readonly IVerificationCodeService _verificationCodeService;
    private readonly ILogger<VerificationCodeController> _logger;
    private readonly ISmsMessageSendingService _smsMessageSendingService;

    public VerificationCodeController(IVerificationCodeService verificationCodeService,
        ILogger<VerificationCodeController> logger, ISmsMessageSendingService smsMessageSendingService)
    {
        _verificationCodeService = verificationCodeService;
        _logger = logger;
        _smsMessageSendingService = smsMessageSendingService;
    }
    
    [HttpPost("send")]
    [Authorize]
    public async Task<IActionResult> SendVerificationCodeAsync([FromBody] PhoneVerificationCode phoneNumber)
    {
        try
        {
            string code = RandomStringGenerator.GenerateNumeric(6);
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            if (userId == null)
            {
                return BadRequest(FormatErrorMessage(UserNotFound, CustomStatusCode.ValueNotFound));
            }

            await _smsMessageSendingService.SendPhoneVerificationCodeAsync(phoneNumber.PhoneNumber, code, CountryCodes.Israel);
            await _verificationCodeService.CreateVerificationCode(userId, new PhoneVerificationCode
            {
                PhoneNumber = phoneNumber.PhoneNumber,
                VerificationCode = code
            });
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending verification code");
            return BadRequest();
        }
    }
    
    private static string RemoveCountryCode(string? phoneNumber)
    {
        if (phoneNumber == null)
            return "";
        if (phoneNumber.StartsWith("972"))
            return phoneNumber.Substring(3);
        return phoneNumber;
    }
    
    [HttpPost("verify")]
    [Authorize]
    public async Task<IActionResult> VerifyVerificationCode([FromBody] PhoneVerificationCode code)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            if (userId == null)
            {
                return BadRequest();
            }

            var verificationCode = await _verificationCodeService.GetVerificationCode(userId);
            if (verificationCode == null)
            {
                return BadRequest(FormatErrorMessage(VerificationCodeNotFound, CustomStatusCode.ValueNotFound));
            }

            if (verificationCode.Expires < DateTime.Now || verificationCode.VerificationCode != code.VerificationCode)
            {
                return BadRequest(FormatErrorMessage(VerificationCodeExpired, CustomStatusCode.VerificationCodeExpired));
            }

            verificationCode.PhoneNumber = RemoveCountryCode(verificationCode.PhoneNumber);

            await _verificationCodeService.ApproveVerificationCode(userId, verificationCode.PhoneNumber);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while verifying verification code");
            return BadRequest();
        }
    }
}
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for verification codes, used to verify phone numbers.
/// Provides a web API and service policy for <see cref="IVerificationCodeService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VerificationCodeController : Controller
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

    /// <summary>
    /// Sends a verification code and saves it in the database.
    /// </summary>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <returns>BadRequest if user somehow got here without being logged in, Ok otherwise.</returns>
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

            await _smsMessageSendingService.SendPhoneVerificationCodeAsync(phoneNumber.PhoneNumber, code,
                CountryCodes.Israel);
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

    /// <summary>
    /// A simple method to remove the country codes from phone numbers in Israel.
    /// </summary>
    /// <param name="phoneNumber">Phone number to remove the code from.</param>
    /// <returns>The substring without the country code.</returns>
    private static string RemoveCountryCode(string? phoneNumber)
    {
        if (phoneNumber == null)
            return "";
        if (phoneNumber.StartsWith("972"))
            return phoneNumber.Substring(3);
        return phoneNumber;
    }

    /// <summary>
    /// Verifies a user's verification code, and lists their number as verified if it is correct.
    /// </summary>
    /// <param name="code">An instance of <see cref="PhoneVerificationCode"/> containing the code.</param>
    /// <returns>BadRequest if the user does not have a verification code of if it already expired, if
    /// they somehow got an empty string into the database as their phone number, if their code already expired,
    /// if the code submitted and the code sent do not match. Ok otherwise.</returns>
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

            if (verificationCode.Expires < DateTime.Now)
            {
                return BadRequest(FormatErrorMessage(VerificationCodeExpired,
                    CustomStatusCode.VerificationCodeExpired));
            }

            if (verificationCode.VerificationCode != code.VerificationCode)
            {
                return BadRequest(FormatErrorMessage(VerificationCodeError, CustomStatusCode.NoMatch));
            }

            verificationCode.PhoneNumber = RemoveCountryCode(verificationCode.PhoneNumber);
            if (string.IsNullOrEmpty(verificationCode.PhoneNumber))
            {
                return BadRequest(FormatErrorMessage(PhoneNumberNotFound, CustomStatusCode.ValueNotFound));
            }

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
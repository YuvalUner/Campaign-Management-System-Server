using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for handling all requests related to the election day.
/// Generally, provides a web API and service policy for <see cref="IElectionDayService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ElectionDayController : Controller
{
    private readonly IElectionDayService _electionDayService;
    private readonly ILogger<ElectionDayController> _logger;

    public ElectionDayController(IElectionDayService electionDayService, ILogger<ElectionDayController> logger)
    {
        _electionDayService = electionDayService;
        _logger = logger;
    }

    /// <summary>
    /// Gets details on the ballot the currently logged in user is assigned to.
    /// </summary>
    /// <returns>Unauthorized if the user is not verified, NotFound if the ballot was not found - which could be an error
    /// on our end or the civil admin's end, or Ok with a JSON object containing the ballot details on success.</returns>
    [HttpGet("ballot")]
    public async Task<IActionResult> GetBallotForUser()
    {
        try
        {
            var isAuthenticated = HttpContext.Session.Get<bool?>(Constants.UserAuthenticationStatus);

            if (isAuthenticated == null || !isAuthenticated.Value)
            {
                return Unauthorized(FormatErrorMessage(VerificationStatusError, CustomStatusCode.VerificationStatusError));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var ballot = await _electionDayService.GetUserAssignedBallot(userId);

            if (ballot == null)
            {
                return NotFound(FormatErrorMessage(NoBallotFound, CustomStatusCode.ValueNotFound));
            }

            return Ok(ballot);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get ballot for user");
            return StatusCode(500, "Error while trying to get ballot for user");
        }
    }
}
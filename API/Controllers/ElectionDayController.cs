using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
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
    
    /// <summary>
    /// API endpoint for updating the vote count for a specific party in a specific ballot.
    /// </summary>
    /// <param name="voteCount"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="increment">True to increment the count, false to decrement.</param>
    /// <returns>Unauthorized if the user does not have permission to update vote counts,
    /// NotFound if the campaign, ballot or party do not exist, Ok otherwise.</returns>
    [HttpPut("vote/{campaignGuid:guid}/{increment:bool}")]
    public async Task<IActionResult> ModifyVoteCount([FromBody] VoteCount voteCount, Guid campaignGuid, bool increment)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.BallotCounting,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var result = await _electionDayService.ModifyVoteCount(voteCount, campaignGuid, increment);

            return result switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, CustomStatusCode.CampaignNotFound)),
                CustomStatusCode.BallotNotFound => NotFound(FormatErrorMessage(BallotNotFound, CustomStatusCode.BallotNotFound)),
                CustomStatusCode.PartyNotFound => NotFound(FormatErrorMessage(PartyNotFound, CustomStatusCode.PartyNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to modify vote count");
            return StatusCode(500, "Error while trying to modify vote count");
        }
    }
    
    /// <summary>
    /// Gets the vote counts across all ballots for a specific campaign.<br/>
    /// Any ballot or party that has no votes will not be included in the result.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns>Unauthorized if the user is not currently in the campaign page, Ok with a list of the vote counts
    /// otherwise.</returns>
    [HttpGet("vote-counts/{campaignGuid:guid}")]
    public async Task<IActionResult> GetVoteCounts(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(AuthorizationError,
                    CustomStatusCode.AuthorizationError));
            }

            var result = await _electionDayService.GetVoteCount(campaignGuid);

            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get vote counts");
            return StatusCode(500, "Error while trying to get vote counts");
        }
    }
}
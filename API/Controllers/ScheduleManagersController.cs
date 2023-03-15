using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for appointing and removing managers for users.
/// Provides a web API and service policy for <see cref="IScheduleManagersService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class ScheduleManagersController : Controller
{
    private readonly IScheduleManagersService _scheduleManagersService;
    private readonly IUsersService _usersService;
    private readonly ILogger<ScheduleManagersController> _logger;

    public ScheduleManagersController(IScheduleManagersService scheduleManagersService, IUsersService usersService,
        ILogger<ScheduleManagersController> logger)
    {
        _scheduleManagersService = scheduleManagersService;
        _usersService = usersService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of the user's managers.
    /// </summary>
    /// <returns>BadRequest if somehow the user got here while not logged in, Ok with a list of the same structure as
    /// <see cref="UserPublicInfo"/>.</returns>
    [HttpGet("get-self-managers")]
    public async Task<IActionResult> GetSelfManagers()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var (statusCode, scheduleManagers) =
                await _scheduleManagersService.GetScheduleManagers(userId: userId.Value);

            // This should never happen.
            // If it does, it means that the user is not logged in, and therefore should not be here.
            // I don't know what my past self was thinking when he wrote this, but my current self is too scared to change it.
            if (statusCode == CustomStatusCode.ParameterMustNotBeNullOrEmpty)
            {
                return BadRequest(FormatErrorMessage(PermissionError, CustomStatusCode.PermissionError));
            }

            // While there is a model with this exact structure, the above method returns user ids as well because
            // other methods use it as well. We don't want to return the user ids, so we create a new object without them.
            var scheduleManagersWithoutUserIds = scheduleManagers.Select(x => new
            {
                x.Email,
                x.DisplayNameEng,
                x.PhoneNumber,
                x.FirstNameHeb,
                x.LastNameHeb,
                x.ProfilePicUrl
            });

            return Ok(scheduleManagersWithoutUserIds);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting self managers");
            return StatusCode(500, "Error while getting self managers");
        }
    }

    /// <summary>
    /// Gets a list of the user's managed users.
    /// </summary>
    /// <returns>Ok with a list of <see cref="UserPublicInfo"/>.</returns>
    [HttpGet("get-managed-users")]
    public async Task<IActionResult> GetManagedUsers()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var managedUsers = await _scheduleManagersService.GetManagedUsers(userId.Value);

            return Ok(managedUsers);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting managed users");
            return StatusCode(500, "Error while getting managed users");
        }
    }

    /// <summary>
    /// Gets a list of all the user's managers.<br/>
    /// This method is meant to be used by managers, to see who else manages the user.
    /// </summary>
    /// <param name="userEmail">Email of the managed user.</param>
    /// <returns>Unauthorized if no email was provided or the user is not a manager of the managed user,
    /// Ok with a list with the same structure as <see cref="UserPublicInfo"/> otherwise.</returns>
    [HttpGet("get-managers/{userEmail}")]
    public async Task<IActionResult> GetManagers(string userEmail)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var (statusCode, scheduleManagers) =
                await _scheduleManagersService.GetScheduleManagers(userEmail: userEmail);

            if (statusCode == CustomStatusCode.ParameterMustNotBeNullOrEmpty)
            {
                return Unauthorized(FormatErrorMessage(EmailNullOrEmpty, CustomStatusCode.ValueCanNotBeNull));
            }

            // If the user is not a manager of the managed user, return unauthorized
            // Only a manager can see who the other managers are.
            if (!scheduleManagers.Any(x => x.UserId == userId))
            {
                return Unauthorized(FormatErrorMessage(PermissionError, CustomStatusCode.PermissionError));
            }

            var scheduleManagersWithoutUserIds = scheduleManagers.Select(x => new
            {
                x.Email,
                x.DisplayNameEng,
                x.PhoneNumber,
                x.FirstNameHeb,
                x.LastNameHeb,
                x.ProfilePicUrl
            });

            return Ok(scheduleManagersWithoutUserIds);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting managers");
            return StatusCode(500, "Error while getting managers");
        }
    }

    /// <summary>
    /// Adds a new manager to the user's list of managers.
    /// </summary>
    /// <param name="userEmail">Email of the new manager to add.</param>
    /// <returns>NotFound if the user was not found, Conflict if user is already a manager of the requesting user,
    /// Ok otherwise.</returns>
    [HttpPost("add-manager/{userEmail}")]
    public async Task<IActionResult> AddManager(string userEmail)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var statusCode = await _scheduleManagersService.AddScheduleManager(userId.Value, userEmail);

            return statusCode switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                CustomStatusCode.DuplicateKey => Conflict(FormatErrorMessage(ManagerAlreadyExists,
                    CustomStatusCode.DuplicateKey)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding manager");
            return StatusCode(500, "Error while adding manager");
        }
    }

    /// <summary>
    /// Removes a manager from the user's list of managers.
    /// </summary>
    /// <param name="userEmail">Email of the manager to remove.</param>
    /// <returns>NotFound if the user is not a manager or does not exist, Ok otherwise.</returns>
    [HttpDelete("remove-manager/{userEmail}")]
    public async Task<IActionResult> RemoveManager(string userEmail)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var statusCode = await _scheduleManagersService.RemoveScheduleManager(userId.Value, userEmail);

            return statusCode switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing manager");
            return StatusCode(500, "Error while removing manager");
        }
    }
}
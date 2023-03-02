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
public class ScheduleManagersController : Controller
{
    private readonly IScheduleManagersService _scheduleManagersService;
    private readonly IUsersService _usersService;
    private readonly ILogger<ScheduleManagersController> _logger;
    
    public ScheduleManagersController(IScheduleManagersService scheduleManagersService, IUsersService usersService, ILogger<ScheduleManagersController> logger)
    {
        _scheduleManagersService = scheduleManagersService;
        _usersService = usersService;
        _logger = logger;
    }

    [HttpGet("get-self-managers")]
    public async Task<IActionResult> GetSelfManagers()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var (statusCode, scheduleManagers) = await _scheduleManagersService.GetScheduleManagers(userId: userId.Value);

            if (statusCode == CustomStatusCode.ParameterMustNotBeNullOrEmpty)
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
            _logger.LogError(e, "Error while getting self managers");
            return StatusCode(500, "Error while getting self managers");
        }
    }

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
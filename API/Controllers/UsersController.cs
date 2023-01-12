﻿using System.Security.Claims;
using DAL.Models;
using DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    
    private readonly IUsersService _usersService;
    private readonly ILogger<UsersController> _logger;
    private readonly IVotersLedgerService _votersLedgerService;
    
    public UsersController(IUsersService usersService, 
        ILogger<UsersController> logger,
        IVotersLedgerService votersLedgerService)
    {
        _usersService = usersService;
        _logger = logger;
        _votersLedgerService = votersLedgerService;
    }

    [Authorize]
    [HttpGet("HomePageInfo")]
    public async Task<IActionResult> HomePageInfo()
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        List<CampaignUser> campaigns = await _usersService.GetUserCampaigns(userId);
        // The CampaignUser model also holds fields that should not be getting out, such as UserId, CampaignId, RoleId.
        // Hence, it needs to be cleaned up first.
        var campaignsCensored = campaigns.Select(x => new
        {
            x.CampaignGuid, x.CampaignName, x.RoleName
        });
        User? user = await _usersService.GetUserPublicInfo(userId);
        // Same goes for the User model - needs to be cleaned up first before sending it out.
        // Else, there is risk of exposing DB schemas and userIds.
        var returnObject = new { User = new
        {
            user.FirstNameEng,
            user.LastNameEng,
            user.DisplayNameEng,
            user.ProfilePicUrl,
            user.FirstNameHeb,
            user.LastNameHeb
        }, Campaigns = campaignsCensored };
        return Ok(returnObject);
    }

    private async Task<bool> VerifyUserPrivateInfo(UserPrivateInfo userInfo)
    {
        // Checking that the user filled in all of the required fields.
        if (userInfo.IdNumber == null 
            || userInfo.FirstNameHeb == null 
            || userInfo.LastNameHeb == null 
            || userInfo.CityName == null)
        {
            return false;
        }
        
        var matchingRecord = await _votersLedgerService.GetSingleVotersLedgerRecord(userInfo.IdNumber);
        // No matching record found in the voters ledger, return false.
        if (matchingRecord == null)
        {
            return false;
        }

        // Check that all the info submitted by the user matches that in the voters ledger.
        // If it is, return true. Else, return false.
        if (userInfo.CityName == matchingRecord.ResidenceName
            && (userInfo.FirstNameHeb == matchingRecord.FirstName || userInfo.FirstNameEng == matchingRecord.FirstName)
            && (userInfo.LastNameHeb == matchingRecord.LastName || userInfo.LastNameEng == matchingRecord.LastName))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Verifies that the user has not already filled out their private info.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<bool> VerifyNonDuplicatePrivateInfo(int? userId)
    {
        var user = await _usersService.GetUserPublicInfo(userId);
        // Really should not happen. Like, ever. But if it does, prevent the user from updating info.
        if (user == null)
        {
            return false;
        }

        // Any user that already had set their private info should have both these fields not null.
        if (user.FirstNameHeb == null && user.LastNameHeb == null)
        {
            return true;
        }

        return false;
    }

    [Authorize]
    [HttpPost("UserPrivateInfo")]
    public async Task<IActionResult> UserPrivateInfo([FromBody] UserPrivateInfo userInfo)
    {
        var userId = HttpContext.Session.GetInt32("userId");
        // Check that the user has not already filled out their private info.
        if (!await this.VerifyNonDuplicatePrivateInfo(userId))
        {
            _logger.LogInformation("User with {UserId} tried to enter duplicate private info", userId);
            return BadRequest();
        }
        // Filling in the info from the Http context
        userInfo.FirstNameEng = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        userInfo.LastNameEng = HttpContext.User.FindFirst(ClaimTypes.Surname)?.Value;
        // Make sure the info the user input matches that in the voters ledger.
        if (!await this.VerifyUserPrivateInfo(userInfo))
        {
            return BadRequest();
        }
        
        // If all checks pass, update the user's info in the database.
        await _usersService.AddUserPrivateInfo(userInfo, userId);
        _logger.LogInformation("User with {UserId} verified their private info", userId);
        return Ok();
    }
}
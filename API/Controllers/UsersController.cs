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
    
    public UsersController(IUsersService usersService, ILogger<UsersController> logger)
    {
        _usersService = usersService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("HomePageInfo")]
    public async Task<IActionResult> HomePageInfo()
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        List<CampaignUser>? campaigns = await _usersService.GetUserCampaigns(userId);
        return Ok(campaigns);
    }
}
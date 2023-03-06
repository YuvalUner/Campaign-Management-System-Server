using API.Utils;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PublicBoardController: Controller
{
    private readonly IPublicBoardService _publicBoardService;
    private readonly ILogger<PublicBoardController> _logger;
    
    public PublicBoardController(IPublicBoardService publicBoardService, ILogger<PublicBoardController> logger)
    {
        _publicBoardService = publicBoardService;
        _logger = logger;
    }
    
    [HttpGet()]
    public async Task<IActionResult> GetPersonalizedPublicBoard([FromQuery] int? limit)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var events = await _publicBoardService.GetEventsForUser(userId, limit);
            var announcements = await _publicBoardService.GetAnnouncementsForUser(userId, limit);
            
            return Ok(new {events, announcements});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting campaign published events");
            return StatusCode(500, "Error while getting campaign published events");
        }
    }
}
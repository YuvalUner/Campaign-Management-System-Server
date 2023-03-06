using API.Utils;
using DAL.Models;
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
    
    [HttpGet("public-board")]
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

    [HttpGet("events-search")]
    public async Task<IActionResult> SearchPublicEvents([FromQuery] EventsSearchParams searchParams)
    {
        try
        {
            var events = await _publicBoardService.SearchEvents(searchParams);
            return Ok(events);   
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while searching public events");
            return StatusCode(500, "Error while searching public events");
        }
    }
    
    [HttpGet("announcements-search")]
    public async Task<IActionResult> SearchPublicAnnouncements([FromQuery] AnnouncementsSearchParams searchParams)
    {
        try
        {
            var announcements = await _publicBoardService.SearchAnnouncements(searchParams);
            return Ok(announcements);   
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while searching public announcements");
            return StatusCode(500, "Error while searching public announcements");
        }
    }
}
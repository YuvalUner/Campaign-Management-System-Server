using API.ExternalProcesses.PythonML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CampaignAdvisorController : Controller
{
    private readonly IPythonMlRunner _pythonMlRunner;
    
    public CampaignAdvisorController(IPythonMlRunner pythonMlRunner)
    {
        _pythonMlRunner = pythonMlRunner;
    }
    
    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] string opponentName)
    {
        var articles = new string[] {"article1", "article2"};
        var targetTweets = new string[] {"tweet1", "tweet2"};
        var tweetsAboutTarget = new string[] {"tweet1", "tweet2"};
        var classifiedTexts = await _pythonMlRunner.RunPythonScript(articles, targetTweets, tweetsAboutTarget);
        return Ok(classifiedTexts);
    }
    
}
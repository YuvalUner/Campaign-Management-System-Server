using API.ExternalProcesses.PythonML;
using API.ExternalProcesses.PythonWebScraping;
using API.Models;
using DAL.DbAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CampaignAdvisorController : Controller
{
    private readonly IPythonMlRunner _pythonMlRunner;
    private readonly IPythonWebscraperRunner _pythonWebscraperRunner;
    
    public CampaignAdvisorController(IPythonMlRunner pythonMlRunner, IPythonWebscraperRunner pythonWebscraperRunner)
    {
        _pythonMlRunner = pythonMlRunner;
        _pythonWebscraperRunner = pythonWebscraperRunner;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] AnalysisParams analysisParams)
    {
        if (string.IsNullOrWhiteSpace(analysisParams.TargetName))
        {
            return BadRequest(FormatErrorMessage(OpponentNameRequired, CustomStatusCode.ValueNullOrEmpty));
        }
        var articles = new List<string> {"article1", "article2"};
        var tweetsCollection = await _pythonWebscraperRunner.RunPythonScript(analysisParams.TargetName,
            analysisParams.TargetTwitterHandle, analysisParams.MaxDays);
        List<string>? targetTweets;
        List<string>? tweetsAboutTarget;
        if (tweetsCollection is null)
        {
            targetTweets = new List<string>();
            tweetsAboutTarget = new List<string>();
        }
        else
        {
            targetTweets = tweetsCollection.TargetTweets;
            tweetsAboutTarget = tweetsCollection.TweetsAboutTarget;
        }
        var classifiedTexts = await _pythonMlRunner.RunPythonScript(articles, targetTweets, tweetsAboutTarget);
        return Ok(classifiedTexts);
    }
    
}
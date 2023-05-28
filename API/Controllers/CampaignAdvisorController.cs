using API.ExternalProcesses.PythonML;
using API.ExternalProcesses.PythonWebScraping;
using API.Models;
using DAL.DbAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;
using Newtonsoft.Json;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CampaignAdvisorController : Controller
{
    private readonly IPythonMlRunner _pythonMlRunner;
    private readonly IPythonWebscraperRunner _pythonWebscraperRunner;
    private readonly string _apiKey;
    
    public CampaignAdvisorController(IPythonMlRunner pythonMlRunner, IPythonWebscraperRunner pythonWebscraperRunner,
        IConfiguration configuration)
    {
        _pythonMlRunner = pythonMlRunner;
        _pythonWebscraperRunner = pythonWebscraperRunner;
        _apiKey = configuration["NewsApiKey"];
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] AnalysisParams analysisParams)
    {
        if (string.IsNullOrWhiteSpace(analysisParams.TargetName))
        {
            return BadRequest(FormatErrorMessage(OpponentNameRequired, CustomStatusCode.ValueNullOrEmpty));
        }
        
        var currentDate = DateTime.Now;
        var startDate = currentDate.AddDays(-14);
        var queryString = $"\"{analysisParams.TargetName}\" AND (says OR said OR declares OR declared OR claims " +
            $"OR claimed OR states OR stated OR announces OR announced)";
        var requestString = $"https://api.newscatcherapi.com/v2/search?q={queryString}&lang=en&search_in=title" +
                            $"&from={startDate.Date:yyyy/MM/dd}&page=1&page_size=100";
        using var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestString),
            Headers =
            {
                { "x-api-key", _apiKey }
            }
        };
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return BadRequest();
        }
        var responseString = await response.Content.ReadAsStringAsync();
        var newsApiResponse = JsonConvert.DeserializeObject<NewsCatcherResponse>(responseString);
        // var request = new EverythingRequest
        // {
        //     Q =  analysisParams.TargetName + " AND (says OR said OR say OR says OR declares OR declared " +
        //         "OR declare OR declares OR announces OR announced OR announce OR announces OR claims OR claimed " +
        //         "OR claim OR claims OR states)",
        //     From = startDate.Date,
        //     Language = Languages.EN,
        //     SortBy = SortBys.Relevancy
        // };
        // var articlesResponse = await _newsApiClient.GetEverythingAsync(request);
        // Get the first 100 article titles
        var articles = newsApiResponse.Articles.Select(article => article.Title).Take(100).ToList();
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
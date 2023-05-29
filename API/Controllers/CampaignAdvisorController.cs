using API.ExternalProcesses.PythonML;
using API.ExternalProcesses.PythonWebScraping;
using API.Models;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
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
    private readonly ICampaignAdvisorAnalysisService _analysisService;
    
    public CampaignAdvisorController(IPythonMlRunner pythonMlRunner, IPythonWebscraperRunner pythonWebscraperRunner,
        IConfiguration configuration, ICampaignAdvisorAnalysisService analysisService)
    {
        _pythonMlRunner = pythonMlRunner;
        _pythonWebscraperRunner = pythonWebscraperRunner;
        _analysisService = analysisService;
        _apiKey = configuration["NewsApiKey"];
    }

    /// <summary>
    /// Aggregates the results of the analysis into a single list of AnalysisRow objects.
    /// </summary>
    /// <param name="analyzedText"></param>
    /// <param name="rowType"></param>
    /// <returns></returns>
    private List<AnalysisRow> AggregateResults(List<TextForAnalysis> analyzedText, RowTypes rowType, Guid resultsGuid)
    {
        // First, find all the unique categories
        var categories = analyzedText.Select(text => text.Topic).Distinct().ToList();
        var analysisRows = new List<AnalysisRow>();

        // Then, for each category, find the relevant texts and aggregate the results
        foreach (var category in categories)
        {
            var relevantTexts = analyzedText.Where(text => text.Topic == category).ToList();
            // Decimal is used to avoid integer division, because that would result in 0 for all percentages
            // This number will always be a natural number, however.
            decimal total = relevantTexts.Count;
            decimal positive = relevantTexts.Count(text => text.Sentiment == "positive");
            decimal negative = relevantTexts.Count(text => text.Sentiment == "negative");
            decimal neutral = relevantTexts.Count(text => text.Sentiment == "neutral");
            decimal hate = relevantTexts.Count(text => text.Hate == "hate");
            analysisRows.Add(new AnalysisRow()
            {
                Topic = category,
                RowType = rowType,
                Hate = hate / total,
                Negative = negative / total,
                Neutral = neutral / total,
                Positive = positive / total,
                // Cast to int to avoid storing the decimal point in the DB
                Total = (int) total,
                ResultsGuid = resultsGuid
            });
        }
        
        return analysisRows;
    }

    private async Task<Guid?> StoreAnalysisResults(AnalysisParams analysisParams, CombinedTextsList analysisResults,
        Guid campaignGuid)
    {
        // First, store the analysis overview in the DB, to prepare for adding the results
        var overview = new AnalysisOverview()
        {
            ResultsTitle = analysisParams.ResultsTitle,
            AnalysisTarget = analysisParams.TargetName,
            TargetTwitterHandle = analysisParams.TargetTwitterHandle,
            MaxDaysBack = analysisParams.MaxDays,
            AdditionalUserRequests = analysisParams.AdditionalUserRequests
        };
        var (statusCode, resultsGuid) = await _analysisService.AddAnalysisOverview(overview, campaignGuid);
        if (statusCode != CustomStatusCode.Ok)
        {
            return null;
        }
        
        // Second, aggregate the results of each category, such that they are all in the AnalysisRow format.
        var articles = AggregateResults(analysisResults.Articles, RowTypes.Article, resultsGuid);
        var tweetsAboutTarget = AggregateResults(analysisResults.TweetsAboutTarget, RowTypes.TweetFromTarget, resultsGuid);
        var targetTweets = AggregateResults(analysisResults.TargetTweets, RowTypes.TweetAboutTarget, resultsGuid);
        
        // Third, add the results to the DB
        await _analysisService.AddAnalysisDetailsRows(articles);
        await _analysisService.AddAnalysisDetailsRows(tweetsAboutTarget); 
        await _analysisService.AddAnalysisDetailsRows(targetTweets);
        
        // Finally, sample 10 articles and 10 tweets from the target, and add them to the DB
        var distinctArticleTitles = analysisResults.Articles.Select(article => article.Text).Distinct().ToList();
        var distinctTweetTexts = analysisResults.TargetTweets.Select(tweet => tweet.Text).Distinct().ToList();
        
        var articleSample = distinctArticleTitles.Take(10).Select(article => new AnalysisSample()
        {
            ResultsGuid = resultsGuid,
            SampleText = article,
            IsArticle = true
        }).ToList();
        
        var tweetSample = distinctTweetTexts.Take(10).Select(tweet => new AnalysisSample()
        {
            ResultsGuid = resultsGuid,
            SampleText = tweet,
            IsArticle = false
        }).ToList();
        
        await _analysisService.AddAnalysisSamples(articleSample);
        await _analysisService.AddAnalysisSamples(tweetSample);
        
        return resultsGuid;
    }

    [HttpPost("analyze/{campaignGuid:guid}")]
    public async Task<IActionResult> Analyze(Guid campaignGuid, [FromBody] AnalysisParams analysisParams)
    {
        if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                new Permission()
                {
                    PermissionTarget = PermissionTargets.CampaignAdvisor,
                    PermissionType = PermissionTypes.Edit
                }))
        {
            return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                CustomStatusCode.PermissionOrAuthorizationError));
        }
        
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
        var resultsGuid = await StoreAnalysisResults(analysisParams, classifiedTexts, campaignGuid);
        if (resultsGuid is null)
        {
            return BadRequest();
        }
        return Ok(new
        {
            resultsGuid
        });
    }
    
}
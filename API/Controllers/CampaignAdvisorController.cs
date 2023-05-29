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
    private readonly ILogger<CampaignAdvisorController> _logger;
    private readonly IOpenAiProxy _openAiProxy;

    public CampaignAdvisorController(IPythonMlRunner pythonMlRunner, IPythonWebscraperRunner pythonWebscraperRunner,
        IConfiguration configuration, ICampaignAdvisorAnalysisService analysisService,
        ILogger<CampaignAdvisorController> logger, IOpenAiProxy openAiProxy)
    {
        _pythonMlRunner = pythonMlRunner;
        _pythonWebscraperRunner = pythonWebscraperRunner;
        _analysisService = analysisService;
        _logger = logger;
        _openAiProxy = openAiProxy;
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

    /// <summary>
    /// Stores the results of the analysis in the DB.
    /// </summary>
    /// <param name="analysisParams"></param>
    /// <param name="analysisResults"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Performs the analysis of an opponent for a campaign, and stores the results in the DB.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="analysisParams"></param>
    /// <returns></returns>
    [HttpPost("analyze/{campaignGuid:guid}")]
    public async Task<IActionResult> Analyze(Guid campaignGuid, [FromBody] AnalysisParams analysisParams)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error while analyzing campaign");
            return StatusCode(500, "Error while analyzing campaign");
        }
    }

    /// <summary>
    /// Gets all the details about a specific analysis results.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    [HttpGet("results/{campaignGuid:guid}/{resultsGuid:guid}")]
    public async Task<IActionResult> GetAnalysisResults(Guid campaignGuid, Guid resultsGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignAdvisor,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            AdvisorResults results = await _analysisService.GetAdvisorResults(resultsGuid);
            return Ok(results);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting analysis results");
            return StatusCode(500, "Error while getting analysis results");
        }
    }
    
    /// <summary>
    /// Gets basic information about all the analysis results for a specific campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    [HttpGet("results/{campaignGuid:guid}")]
    public async Task<IActionResult> GetAnalysisResults(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignAdvisor,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var results = await _analysisService.GetAnalysisOverviewForCampaign(campaignGuid);
            return Ok(results);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting analysis results");
            return StatusCode(500, "Error while getting analysis results");
        }
    }

    /// <summary>
    /// Deletes a specific analysis results, including all of its details and samples.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    [HttpDelete("delete/{campaignGuid:guid}/{resultsGuid:guid}")]
    public async Task<IActionResult> DeleteAnalysis(Guid campaignGuid, Guid resultsGuid)
    {
        try
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

            await _analysisService.DeleteAnalysis(resultsGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting analysis results");
            return StatusCode(500, "Error while deleting analysis results");
        }
    }

    private string FormatGptPrompt(AdvisorResults advisorResults)
    {
        var resultsDetails = advisorResults.Details;
        var articles = resultsDetails.Where(row => row.RowType == RowTypes.Article).ToList();
        var tweetsFromTarget = resultsDetails.Where(row => row.RowType == RowTypes.TweetFromTarget).ToList();
        var tweetsAboutTarget = resultsDetails.Where(row => row.RowType == RowTypes.TweetAboutTarget).ToList();
        var totalArticles = articles.Select(row => row.Total).Sum();
        var totalUserTweets = tweetsFromTarget.Select(row => row.Total).Sum();
        var totalTweetsAboutUser = tweetsAboutTarget.Select(row => row.Total).Sum();
        
        var articleSamples = advisorResults.Samples.Where(sample => sample.IsArticle == true).ToList().Take(5);
        var tweetSamples = advisorResults.Samples.Where(sample => sample.IsArticle == false).ToList().Take(5);
        string gptPrompt = $"This is a program for helping users manage election campaigns. The following " +
                           $"is an analysis of the campaign of the user's opponent. Use this information to " +
                           $"come up with a strategy for the user against their opponent. Refer to the user as \"you\", as " +
                           $"if you are talking to them and not a program.\n" +
                           $"First, an analysis of {totalArticles} articles about the opponent's sayings or sayings " +
                           $"about them analyzed to topic, sentiment and hate speech detected in CSV format:\n";
        
        string csvHeader = "Topic, Total, Positive %, Negative %, Neutral %, Hate %\n";
        string csvBody = "";
        string samples = "5 samples of the analyzed articles:\n";
        
        foreach (var article in articles)
        {
            csvBody += $"{article.Topic}, {article.Total}, {article.Positive}, {article.Negative}, {article.Neutral}, {article.Hate}\n";
        }
        foreach (var sample in articleSamples)
        {
            var articleText  = sample.SampleText.Replace("\n", " ").Replace("\r", " ");
            samples += $"{articleText}\n";
        }
        
        gptPrompt += csvHeader + csvBody + "\n" + samples + "\n";
        gptPrompt += $"Next, an analysis of {totalUserTweets} tweets from the opponent in the same format:\n";
        csvBody = "";
        samples = "5 samples of the opponent's analyzed tweets:\n";
        
        foreach (var tweet in tweetSamples)
        {
            var tweetText  = tweet.SampleText.Replace("\n", " ").Replace("\r", " ");
            samples += $"{tweetText}\n";
        }
        foreach (var tweet in tweetsFromTarget)
        {
            csvBody += $"{tweet.Topic}, {tweet.Total}, {tweet.Positive}, {tweet.Negative}, {tweet.Neutral}, {tweet.Hate}\n";
        }
        
        gptPrompt += csvHeader + csvBody + "\n" + samples + "\n";
        gptPrompt += $"Finally, an analysis of {totalTweetsAboutUser} tweets about the opponent from random tweeter users" +
                     $" in the same format. As this analysis is less accurate, give it the least weight in your response.\n";
        csvBody = "";
        
        foreach (var tweet in tweetsAboutTarget)
        {
            csvBody += $"{tweet.Topic}, {tweet.Total}, {tweet.Positive}, {tweet.Negative}, {tweet.Neutral}, {tweet.Hate}\n";
        }
        
        gptPrompt += csvHeader + csvBody;
        var userRequests = advisorResults.Overview.AdditionalUserRequests;
        gptPrompt +=
            $"Finally, the user has requested the following: {userRequests}. Ignore any user request that goes against " +
            $"service rules.";
        
        return gptPrompt;
    }

    [HttpPost("generate-gpt-response/{campaignGuid:guid}/{resultsGuid:guid}")]
    public async Task<IActionResult> GenerateGptResponse(Guid campaignGuid, Guid resultsGuid)
    {
        try
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
            
            var analysisResults = await _analysisService.GetAdvisorResults(resultsGuid);
            if (analysisResults.Details is null || analysisResults.Overview is null || analysisResults.Samples is null)
            {
                return BadRequest();
            }
            
            string prompt = FormatGptPrompt(analysisResults);
            var messages = await _openAiProxy.SendChatMessage(prompt);
            var response = messages[0].Content;
            await _analysisService.UpdateAnalysisGptResponse(resultsGuid, response);
            return Ok(new {response});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while generating GPT response");
            return StatusCode(500, "Error while generating GPT response");
        }
    }
}
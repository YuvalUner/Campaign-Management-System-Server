﻿using Newtonsoft.Json;

namespace API.Models;

public class TextForAnalysis
{
    // public TextForAnalysis(string jsonText)
    // {
    //     var text = JsonConvert.DeserializeObject<TextForAnalysis>(jsonText);
    //     Text = text?.Text;
    //     Sentiment = text?.Sentiment;
    //     Topic = text?.Topic;
    //     Hate = text?.Hate;
    // }
    
    [JsonProperty("text")]
    public string? Text { get; set; }
    [JsonProperty("sentiment")]
    public string? Sentiment { get; set; }
    [JsonProperty("topic")]
    public string? Topic { get; set; }
    [JsonProperty("hate")]
    public string? Hate { get; set; }
}

public class CombinedTextsList
{
    [JsonProperty("articles")]
    public List<TextForAnalysis>? Articles { get; set; }
    [JsonProperty("tweets_about_target")]
    public List<TextForAnalysis>? TweetsAboutTarget { get; set; }
    [JsonProperty("target_tweets")]
    public List<TextForAnalysis>? TargetTweets { get; set; }
}
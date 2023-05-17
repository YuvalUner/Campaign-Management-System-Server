using Newtonsoft.Json;

namespace API.Models;

public class TweetsCollection
{
    [JsonProperty("target_tweets")]
    public List<string>? TargetTweets { get; set; }
    [JsonProperty("tweets_about_target")]
    public List<string>? TweetsAboutTarget { get; set; }
}
using Newtonsoft.Json;

namespace API.Models;

public class ArticleResult
{
    [JsonProperty("title")]
    public string? Title { get; set; }
}
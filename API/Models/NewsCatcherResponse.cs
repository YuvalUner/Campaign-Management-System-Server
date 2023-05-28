using Newtonsoft.Json;

namespace API.Models;

public class NewsCatcherResponse
{
    [JsonProperty("articles")]
    public ArticleResult[]? Articles { get; set; }
    [JsonProperty("status")]
    public string? Status { get; set; }
    [JsonProperty("total_hits")]
    public int? TotalHits { get; set; }
    [JsonProperty("total_pages")]
    public int? TotalPages { get; set; }
    [JsonProperty("page")]
    public int? Page { get; set; }
    [JsonProperty("page_size")]
    public int? PageSize { get; set; }
}
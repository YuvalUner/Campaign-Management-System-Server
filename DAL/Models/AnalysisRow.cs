namespace DAL.Models;

public enum RowTypes
{
    Article = 0,
    TweetFromTarget = 1,
    TweetAboutTarget = 2
}

public class AnalysisRow
{
    public Guid? ResultsGuid { get; set; }
    public string? Topic { get; set; }
    public int? Total { get; set; }
    public Decimal? Positive { get; set; }
    public Decimal? Negative { get; set; }
    public Decimal? Neutral { get; set; }
    public Decimal? Hate { get; set; }
    public RowTypes? RowType { get; set; }
}
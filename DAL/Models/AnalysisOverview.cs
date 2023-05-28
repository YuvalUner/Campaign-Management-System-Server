namespace DAL.Models;

public class AnalysisOverview
{
    public Guid? ResultsGuid { get; set; }
    public DateTime? TimePerformed { get; set; }
    public string? ResultsTitle { get; set; }
    public string? AnalysisTarget { get; set; }
    public string? TargetTwitterHandle { get; set; }
    public int? MaxDaysBack { get; set; }
    public string? GptResponse { get; set; }
    public string? AdditionalUserRequests { get; set; }
}
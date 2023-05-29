namespace API.Models;

public class AnalysisParams
{
    public string? TargetName { get; set; }
    public string? TargetTwitterHandle { get; set; }
    public int? MaxDays { get; set; }
    
    public string? AdditionalUserRequests { get; set; }
    public string? ResultsTitle { get; set; }
}
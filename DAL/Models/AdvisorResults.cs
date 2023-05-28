namespace DAL.Models;

public class AdvisorResults
{
    public AnalysisOverview? Overview { get; set; }
    public List<AnalysisRow>? Details { get; set; }
    public List<AnalysisSample>? Samples { get; set; }
}
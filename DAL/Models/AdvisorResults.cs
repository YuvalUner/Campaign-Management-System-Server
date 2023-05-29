namespace DAL.Models;

public class AdvisorResults
{
    public AnalysisOverview? Overview { get; set; }
    public IEnumerable<AnalysisRow>? Details { get; set; }
    public IEnumerable<AnalysisSample>? Samples { get; set; }
}
namespace DAL.Models;

public static class BuiltInJobTypes
{
    public const string PhoneOperator = "Phone Operator";
    public const string BallotCrew = "Ballot Crew";
    public const string BallotStaff = "Ballot Staff";
    public const string Driver = "Driver";
    public const string Other = "Other";

    private static string[] All = new[]
    {
        PhoneOperator,
        BallotCrew,
        BallotStaff,
        Driver,
        Other
    };
    
    public static bool IsBuiltIn(string jobTypeName)
    {
        return All.Contains(jobTypeName);
    }
}

public class JobType
{
    public string? JobTypeName { get; set; }
    public string? JobTypeDescription { get; set; }
}
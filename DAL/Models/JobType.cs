namespace DAL.Models;

/// <summary>
/// A static class made to contain all the built-in job types.<br/>
/// Made to be used in the <see cref="JobType"/> model, to prevent constantly retrieving this static list from
/// the database.<br/>
/// </summary>
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
    
    /// <summary>
    /// A method to check if a given job type name is a built-in job type.<br/>
    /// </summary>
    /// <param name="jobTypeName">The name of the job type to check.</param>
    /// <returns>True if the job type is a built in one, false otherwise.</returns>
    public static bool IsBuiltIn(string jobTypeName)
    {
        return All.Contains(jobTypeName);
    }
}

/// <summary>
/// A model for the job_types table.<br/>
/// Contains only the fields that are relevant to the client.<br/>
/// </summary>
public class JobType
{
    public string? JobTypeName { get; set; }
    public string? JobTypeDescription { get; set; }
    
    public bool IsCustomJobType { get; set; }
}
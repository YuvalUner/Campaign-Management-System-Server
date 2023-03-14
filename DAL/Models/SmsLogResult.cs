namespace DAL.Models;

using DbAccess;

/// <summary>
/// A model for a single row of the <see cref="StoredProcedureNames.GetBaseSmsLogs"/> stored procedure.<br/>
/// Each row represents a single SMS that was sent.<br/>
/// </summary>
public class SmsLogResult
{
    public Guid MessageGuid { get; set; }
    public string? MessageContents { get; set; }
    public DateTime MessageDate { get; set; }
    public int SentCount { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? ProfilePicUrl { get; set; }
}
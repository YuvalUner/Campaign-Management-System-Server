namespace DAL.Models;

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
namespace DAL.Models;

/// <summary>
/// A model for the public information of a user.<br/>
/// This model can be exposed to other users where needed (assuming they have permission to see it).
/// </summary>
public class UserPublicInfo
{
    public string? Email { get; set; }
    public string? FirstNameEng { get; set; }
    public string? LastNameEng { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? PhoneNumber { get; set; }
}
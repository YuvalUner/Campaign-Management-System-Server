namespace DAL.Models;

/// <summary>
/// An extended version of <see cref="CustomEvent"/> that includes the publisher's details.<br/>
/// </summary>
public class EventWithCreatorDetails: CustomEvent
{
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
namespace DAL.Models;

public class User
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstNameEng { get; set; }
    public string? LastNameEng { get; set; }
    public int? IdNum { get; set; }
    public string? DisplayNameEng { get; set; }
    
    public string? ProfilePicUrl { get; set; }
    
    public string? FirstNameHeb { get; set; }
    
    public string? LastNameHeb { get; set; }
    
    public bool Authenticated { get; set; }
    
    public IEnumerable<Campaign?>? Campaigns { get; set; }
}
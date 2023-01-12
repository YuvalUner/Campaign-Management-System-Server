namespace DAL.Models;

public class UserPrivateInfo
{
    public int? IdNumber { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? CityName { get; set; }
    // Fallback parameters, due to our database having English names too.
    // In a real scenario, they should not be needed.
    public string? FirstNameEng { get; set; }
    public string? LastNameEng { get; set; }
}
namespace DAL.Models;

public class UpdateSupportStatusParams
{
    public int IdNum { get; set; }
    
    /// <summary>
    /// null = unknown, 0 = no, 1 = yes
    /// </summary>
    public bool? SupportStatus { get; set; }
}
namespace DAL.Models;

/// <summary>
/// A model for the parameters needed when updating a person's support status for a campaign.<br/>
/// </summary>
public class UpdateSupportStatusParams
{
    public int IdNum { get; set; }

    /// <summary>
    /// null = unknown, 0 = no, 1 = yes
    /// </summary>
    public bool? SupportStatus { get; set; }
}
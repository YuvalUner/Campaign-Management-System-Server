namespace DAL.Models;

/// <summary>
/// A model for external authentication.<br/>
/// </summary>
public class ExternalAuthDto
{
    /// <summary>
    /// Who the provider of the external authentication is.<br/>
    /// </summary>
    public string? Provider { get; set; }
    
    /// <summary>
    /// The access token of the external authentication.<br/>
    /// </summary>
    public string? IdToken { get; set; }
}
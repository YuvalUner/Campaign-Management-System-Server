namespace DAL.Models;

/// <summary>
/// A model for a single row of the users table in the database.<br/>
/// </summary>
public class User
{
    /// <summary>
    /// The primary key of the table, an auto-incrementing integer.<br/>
    /// Absolutely not to be exposed to the user.<br/>
    /// </summary>
    public int UserId { get; set; }

    public string? Email { get; set; }
    public string? FirstNameEng { get; set; }
    public string? LastNameEng { get; set; }

    /// <summary>
    /// The user's Id Number, the one that is given to them by the government.<br/>
    /// </summary>
    public int? IdNum { get; set; }

    public string? DisplayNameEng { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }

    /// <summary>
    /// Whether a user authenticated their private info or not.<br/>
    /// Authenticating means that we managed to verify their identity again the voter's ledger.<br/>
    /// </summary>
    public bool Authenticated { get; set; }

    public string? PhoneNumber { get; set; }
}
namespace DAL.Models;

using DbAccess;

/// <summary>
/// A model for the result of the <see cref="StoredProcedureNames.FilterVotersLedger"/> stored procedure, and an extension
/// of <see cref="VotersLedgerRecord"/>.<br/>
/// Each instance of this class represents a single record from the voters ledger table, with the addition of the user's
/// phone number(s), email address(es), support status for a campaign, and the ballot assigned to them.
/// </summary>
public class VotersLedgerFilterRecord : VotersLedgerRecord
{
    public string? Email1 { get; set; }
    public string? Email2 { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public double? InnerCityBallotId { get; set; }
    public string? BallotAddress { get; set; }
    public string? BallotLocation { get; set; }
    public bool? Accessible { get; set; }
    public int? ElligibleVoters { get; set; }
    public bool? SupportStatus { get; set; }
}
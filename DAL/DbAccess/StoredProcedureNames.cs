namespace DAL.DbAccess;

public static class StoredProcedureNames
{
    /// <summary>
    /// Tests the connection to the database by executing a stored procedure that returns a single value.
    /// </summary>
    public static readonly string TestConnection = "usp_TestConnection";

    
    /// <summary>
    /// Gets a user's info by their email.<br/>
    /// Params: email (string)
    /// </summary>
    public static readonly string GetAllUserInfoByEmail = "usp_GetAllUserInfoByEmail";

    /// <summary>
    /// Adds a user to the users table.<br/>
    /// Params: email (String), firstNameEng (string), lastNameEng (string), displayNameEng(str), profilePicUrl (str)
    /// </summary>
    public static readonly string CreateUser = "usp_InsertUser";

    /// <summary>
    /// Adds a campaign to the campaigns table.<br/>
    /// Params: campaignName (string), campaignCreatorUserId (int), campaignDescription (string).
    /// </summary>
    public static readonly string AddCampaign = "usp_InsertCampaign";
    
    /// <summary>
    /// Links a user to a campaign by adding a row to the campaign_users table.<br/>
    /// Params: campaignId (int), userId (int)
    /// </summary>
    public static readonly string LinkUserToCampaign = "usp_LinkUserToCampaign";

    /// <summary>
    /// Gets public info - name in English and Hebrew and profile picture by a user's user id.<br/>
    /// Params: userId (int)
    /// </summary>
    public static readonly string GetUserPublicInfoByUserID = "usp_GetUserPublicInfoById";

    /// <summary>
    /// Gets all campaigns the user is a part of, along with their role in the campaign.<br/>
    /// Params: userId (int).
    /// </summary>
    public static readonly string GetUserCampaigns = "usp_GetUserCampaigns";
    
    /// <summary>
    /// Gets a single row from the voters ledger by the voter's ID number.<br/>
    /// Params: voterId (int)
    /// </summary>
    public static readonly string GetFromVotersLedgerById = "usp_GetFromVotersLedgerByIdNum";

    /// <summary>
    /// Adds the user's private info to the users and dynamic ledger tables.<br/>
    /// Params: userId (int), firstNameHeb (string), lastNameHeb (string), idNum (int).
    /// </summary>
    public static readonly string AddUserPrivateInfo = "usp_InsertUserPrivateInfo";

    /// <summary>
    /// Gets whether the user is authenticated or not.<br/>
    /// Params: userId (int).
    /// </summary>
    public static readonly string GetUserAuthenticationStatus = "usp_GetUserAuthenticationStatus";

    /// <summary>
    /// Modifies a campaign's info in the database. <br/>
    /// Params: campaignGuid (Guid), campaignDescription (string?), campaignLogoUrl (string?).
    /// </summary>
    public static readonly string ModifyCampaign = "usp_UpdateCampaign";
    
    /// <summary>
    /// Gets the Guid of a campaign by its id.<br/>
    /// Params: campaignId (int)
    /// </summary>
    public static readonly string GetGuidByCampaignId = "usp_GetCampaignGuidById";
}
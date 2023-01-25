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
    /// Params: email (String), firstNameEng (string), lastNameEng (string), displayNameEng(str), profilePicUrl (str)<br/>
    /// Returns: newly created user's id (int)
    /// </summary>
    public static readonly string CreateUser = "usp_InsertUser";

    /// <summary>
    /// Adds a campaign to the campaigns table.<br/>
    /// Params: campaignName (string), campaignCreatorUserId (int), campaignDescription (string).<br/>
    /// Returns: newly created campaign's id (int)
    /// </summary>
    public static readonly string AddCampaign = "usp_InsertCampaign";
    
    /// <summary>
    /// Links a user to a campaign by adding a row to the campaign_users table.<br/>
    /// Params: campaignGuid (Guid), userId (int)
    /// </summary>
    public static readonly string LinkUserToCampaign = "usp_LinkUserToCampaign";

    /// <summary>
    /// Gets public info - name in English and Hebrew and profile picture by a user's user id.<br/>
    /// Params: userId (int)
    /// </summary>
    public static readonly string GetUserPublicInfoByUserId = "usp_GetUserPublicInfoById";

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
    /// Returns: 1 on success, -1 on failure.
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

    /// <summary>
    /// Creates a new invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public static readonly string CreateCampaignInvite = "usp_SetCampaignInviteGuid";
    
    /// <summary>
    /// Revokes an invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public static readonly string RevokeCampaignInvite = "usp_DeleteCampaignInviteGuid";
    
    /// <summary>
    /// Gets the invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public static readonly string GetCampaignInviteGuid = "usp_GetCampaignInviteGuid";
    
    /// <summary>
    /// Gets the campaign's GUID by its invite GUID.<br/>
    /// Params: campaignInviteGuid (Guid)
    /// </summary>
    public static readonly string GetCampaignGuidByInviteGuid = "usp_GetCampaignGuidByInviteGuid";

    /// <summary>
    /// Checks whether or not a user is in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).<br/>
    /// Returns: 1 if the user is in the campaign, 0 if not.
    /// </summary>
    public static readonly string IsUserInCampaign = "usp_IsUserInCampaign";

    /// <summary>
    /// Gets filtered results from the voters ledger, by the filter's parameters.<br/>
    /// Params: campaignGuid (Guid), idNum (int) optional, cityName (string) optional,
    /// streetName (string) optional, ballotId (float) optional, supportStatus (bool) optional,
    /// firstName (string) optional, lastName (string) optional.
    /// </summary>
    public static readonly string FilterVotersLedger = "usp_GetFilteredVotersLedgerRecords";

    /// <summary>
    /// Gets the type and city of a campaign by its GUID.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public static readonly string GetCampaignType = "usp_GetCampaignTypeAndCityByGuid";
    
    /// <summary>
    /// Deletes a user from the database.<br/>
    /// Params: userId (int)
    /// </summary>
    public static readonly string DeleteUser = "usp_DeleteUser";
    
    /// <summary>
    /// Adds a permission to a user in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), permissionType (string), permissionForScreen (string).
    /// </summary>
    public static readonly string AddPermission = "usp_AddPermission";

    /// <summary>
    /// Gets the user's permission set for a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).
    /// </summary>
    public static readonly string GetPermissions = "usp_GetUserPermissionSet";
    
    /// <summary>
    /// Removes a permission from the user in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), permissionType (string), permissionForScreen (string).
    /// </summary>
    public static readonly string RemovePermission = "usp_RemovePermission";

    /// <summary>
    /// Gets the role a user is assigned to in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).
    /// </summary>
    public static readonly string GetUserRoleInCampaign = "usp_GetUserRole";
    
    /// <summary>
    /// Gets the list of users in a campaign.<br/>
    /// Params: campaignGuid (Guid).
    /// </summary>
    public static readonly string GetUsersInCampaign = "usp_GetCampaignUsers";

    /// <summary>
    /// Updates a user's support status for a campaign.<br/>
    /// Params: voterId (int), campaignGuid (Guid), supportStatus (bool).
    /// If support status is unknown, pass null.<br/>
    /// Returns: 1 on success, -1 on failure. Failure could be due to mismatch of campaign city and voter city.
    /// </summary>
    public static readonly string UpdateSupportStatus = "usp_UpdateSupportStatus";
    
    /// <summary>
    /// Deletes a campaign.
    /// Params: campaignGuid (Guid)
    /// </summary>
    public static readonly string DeleteCampaign = "usp_DeleteCampaign";
    
    /// <summary>
    /// Adds a phone number to a user, and also updates their record in the voters ledger.<br/>
    /// Params: userId (int), phoneNumber (string)
    /// </summary>
    public static readonly string AddUserPhoneNumber = "usp_AddUserPhoneNumber";
    
    /// <summary>
    /// Gets the email and phone number of a user.<br/>
    /// Params: userId (int)
    /// </summary>
    public static readonly string GetUserContactInfo = "usp_GetUserContactInfo";
    
    /// <summary>
    /// Modifies a user's notification settings for when someone joins a campaign.<br/>
    /// Params: userId (int), campaignGuid (Guid), viaEmail (bool), viaSms (bool)<br/>
    /// To remove the user from the notification list, pass null for both viaEmail and viaSms.
    /// </summary>
    public static readonly string ModifyUserToNotify = "usp_ModifyUserToNotify";
    
    /// <summary>
    /// Adds a verification code for a user's phone number.<br/>
    /// Params: userId (int), phoneNumber (string), verificationCode (string)
    /// </summary>
    public static readonly string AddVerificationCode = "usp_AddVerificationCode";
    
    /// <summary>
    /// Gets a verification code for a user's phone number.<br/>
    /// Params: userId (int)
    /// </summary>
    public static readonly string GetVerificationCode = "usp_GetVerificationCode";
    
    /// <summary>
    /// Approves a user's phone number if the server completed its checks.<br/>
    /// Deletes the verification code from the database and adds it to the user.<br/>
    /// Params: userId (int), phoneNumber (string)
    /// </summary>
    public static readonly string ApproveVerificationCode = "usp_ApproveVerificationCode";
    
    /// <summary>
    /// Gets a list of users with their contact details and notification settings for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public static readonly string GetUsersToNotify = "usp_GetUsersToNotify";
}
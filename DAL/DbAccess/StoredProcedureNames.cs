namespace DAL.DbAccess;

public static class StoredProcedureNames
{
    /// <summary>
    /// Tests the connection to the database by executing a stored procedure that returns a single value.
    /// </summary>
    public const string TestConnection = "usp_TestConnection";

    
    /// <summary>
    /// Gets a user's info by their email.<br/>
    /// Params: email (string)
    /// </summary>
    public const string GetAllUserInfoByEmail = "usp_GetAllUserInfoByEmail";

    /// <summary>
    /// Adds a user to the users table.<br/>
    /// Params: email (String), firstNameEng (string), lastNameEng (string), displayNameEng(str), profilePicUrl (str)<br/>
    /// Returns: newly created user's id (int)
    /// </summary>
    public const string CreateUser = "usp_InsertUser";

    /// <summary>
    /// Adds a campaign to the campaigns table.<br/>
    /// Params: campaignName (string), campaignCreatorUserId (int), campaignDescription (string).<br/>
    /// Returns: newly created campaign's id (int)
    /// </summary>
    public const string AddCampaign = "usp_InsertCampaign";
    
    /// <summary>
    /// Links a user to a campaign by adding a row to the campaign_users table.<br/>
    /// Params: campaignGuid (Guid), userId (int)
    /// </summary>
    public const string LinkUserToCampaign = "usp_LinkUserToCampaign";

    /// <summary>
    /// Gets public info - name in English and Hebrew and profile picture by a user's user id.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string GetUserPublicInfoByUserId = "usp_GetUserPublicInfoById";

    /// <summary>
    /// Gets all campaigns the user is a part of, along with their role in the campaign.<br/>
    /// Params: userId (int).
    /// </summary>
    public const string GetUserCampaigns = "usp_GetUserCampaigns";
    
    /// <summary>
    /// Gets a single row from the voters ledger by the voter's ID number.<br/>
    /// Params: voterId (int)
    /// </summary>
    public const string GetFromVotersLedgerById = "usp_GetFromVotersLedgerByIdNum";

    /// <summary>
    /// Adds the user's private info to the users and dynamic ledger tables.<br/>
    /// Params: userId (int), firstNameHeb (string), lastNameHeb (string), idNum (int).
    /// Returns: 1 on success, -1 on failure.
    /// </summary>
    public const string AddUserPrivateInfo = "usp_InsertUserPrivateInfo";

    /// <summary>
    /// Gets whether the user is authenticated or not.<br/>
    /// Params: userId (int).
    /// </summary>
    public const string GetUserAuthenticationStatus = "usp_GetUserAuthenticationStatus";

    /// <summary>
    /// Modifies a campaign's info in the database. <br/>
    /// Params: campaignGuid (Guid), campaignDescription (string?), campaignLogoUrl (string?).
    /// </summary>
    public const string ModifyCampaign = "usp_UpdateCampaign";
    
    /// <summary>
    /// Gets the Guid of a campaign by its id.<br/>
    /// Params: campaignId (int)
    /// </summary>
    public const string GetGuidByCampaignId = "usp_GetCampaignGuidById";

    /// <summary>
    /// Creates a new invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string CreateCampaignInvite = "usp_SetCampaignInviteGuid";
    
    /// <summary>
    /// Revokes an invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string RevokeCampaignInvite = "usp_DeleteCampaignInviteGuid";
    
    /// <summary>
    /// Gets the invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetCampaignInviteGuid = "usp_GetCampaignInviteGuid";
    
    /// <summary>
    /// Gets the campaign's GUID by its invite GUID.<br/>
    /// Params: campaignInviteGuid (Guid)
    /// </summary>
    public const string GetCampaignByInviteGuid = "usp_GetCampaignByInviteGuid";

    /// <summary>
    /// Checks whether or not a user is in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).<br/>
    /// Returns: 1 if the user is in the campaign, 0 if not.
    /// </summary>
    public const string IsUserInCampaign = "usp_IsUserInCampaign";

    /// <summary>
    /// Gets filtered results from the voters ledger, by the filter's parameters.<br/>
    /// Params: campaignGuid (Guid), idNum (int) optional, cityName (string) optional,
    /// streetName (string) optional, ballotId (float) optional, supportStatus (bool) optional,
    /// firstName (string) optional, lastName (string) optional.
    /// </summary>
    public const string FilterVotersLedger = "usp_GetFilteredVotersLedgerRecords";

    /// <summary>
    /// Gets the type and city of a campaign by its GUID.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetCampaignType = "usp_GetCampaignTypeAndCityByGuid";
    
    /// <summary>
    /// Deletes a user from the database.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string DeleteUser = "usp_DeleteUser";
    
    /// <summary>
    /// Adds a permission to a user in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), permissionType (string), permissionForScreen (string).
    /// </summary>
    public const string AddPermission = "usp_AddPermission";

    /// <summary>
    /// Gets the user's permission set for a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).
    /// </summary>
    public const string GetPermissions = "usp_GetUserPermissionSet";
    
    /// <summary>
    /// Removes a permission from the user in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), permissionType (string), permissionForScreen (string).
    /// </summary>
    public const string RemovePermission = "usp_RemovePermission";

    /// <summary>
    /// Gets the role a user is assigned to in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).
    /// </summary>
    public const string GetUserRoleInCampaign = "usp_GetUserRole";
    
    /// <summary>
    /// Gets the list of users in a campaign.<br/>
    /// Params: campaignGuid (Guid).
    /// </summary>
    public const string GetUsersInCampaign = "usp_GetCampaignUsers";

    /// <summary>
    /// Updates a user's support status for a campaign.<br/>
    /// Params: voterId (int), campaignGuid (Guid), supportStatus (bool).
    /// If support status is unknown, pass null.<br/>
    /// Returns: 1 on success, -1 on failure. Failure could be due to mismatch of campaign city and voter city.
    /// </summary>
    public const string UpdateSupportStatus = "usp_UpdateSupportStatus";
    
    /// <summary>
    /// Deletes a campaign.
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string DeleteCampaign = "usp_DeleteCampaign";

    /// <summary>
    /// Adds a phone number to a user, and also updates their record in the voters ledger.<br/>
    /// Params: userId (int), phoneNumber (string)
    /// </summary>
    public const string AddUserPhoneNumber = "usp_AddUserPhoneNumber";

    /// <summary>
    /// Gets the email and phone number of a user.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string GetUserContactInfo = "usp_GetUserContactInfo";

    /// <summary>
    /// Modifies a user's notification settings for when someone joins a campaign.<br/>
    /// Params: userId (int), campaignGuid (Guid), viaEmail (bool), viaSms (bool)<br/>
    /// To remove the user from the notification list, pass null for both viaEmail and viaSms.
    /// </summary>
    public const string ModifyUserToNotify = "usp_ModifyUserToNotify";

    /// <summary>
    /// Adds a verification code for a user's phone number.<br/>
    /// Params: userId (int), phoneNumber (string), verificationCode (string)
    /// </summary>
    public const string AddVerificationCode = "usp_AddVerificationCode";

    /// <summary>
    /// Gets a verification code for a user's phone number.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string GetVerificationCode = "usp_GetVerificationCode";

    /// <summary>
    /// Approves a user's phone number if the server completed its checks.<br/>
    /// Deletes the verification code from the database and adds it to the user.<br/>
    /// Params: userId (int), phoneNumber (string)
    /// </summary>
    public const string ApproveVerificationCode = "usp_ApproveVerificationCode";

    /// <summary>
    /// Gets a list of users with their contact details and notification settings for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetUsersToNotify = "usp_GetUsersToNotify";

    /// <summary>
    /// Gets a list of all the roles in a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetCampaignRoles = "usp_GetCampaignRoles";

    /// <summary>
    /// Adds a custom role to a campaign.<br/>
    /// Params: campaignGuid (Guid), roleName (string), roleDescription (string)<br/>
    /// Returns: 1 on success, -1 on failure due to duplicate role name in campaign,
    /// -2 on failure due to too many roles in campaign.
    /// </summary>
    public const string AddCustomRole = "usp_AddCustomRole";

    /// <summary>
    /// Assigns a user to an administrative role and gives them full permissions within the campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), roleName (string)<br/>
    /// Returns: 1 on success, -1 if the role name is invalid.
    /// </summary>
    public const string AssignUserToAdministrativeRole = "usp_AssignUserToAdministrativeRole";

    /// <summary>
    /// Assigns a user to a normal, non administrative role.<br/>
    /// Params: campaignGuid (Guid), userId (int), roleName (string)<br/>
    /// Returns: 1 on success, -1 if the role name is invalid.
    /// </summary>
    public const string AssignUserToRole = "usp_AssignUserToRole";

    /// <summary>
    /// Deletes a role from a campaign.<br/>
    /// Params: campaignGuid (Guid), roleName (string)<br/>
    /// </summary>
    public const string DeleteRole = "usp_DeleteRole";

    /// <summary>
    /// Updates a role's description.<br/>
    /// Params: campaignGuid (Guid), roleName (string), roleDescription (string)<br/>
    /// </summary>
    public const string UpdateRole = "usp_UpdateRole";


    /// <summary>
    /// Removes a user from their role and sets them back to the volunteer role.<br/>
    /// Params: campaignGuid (Guid), userEmail (string)<br/>
    /// </summary>
    public const string RemoveUserFromRole = "usp_RemoveFromRole";

    /// <summary>
    /// Gets a single role by its name and campaign.<br/>
    /// Params: campaignGuid (Guid), roleName (string)<br/>
    /// </summary>
    public const string GetRole = "usp_GetRole";

    /// <summary>
    /// Removes a user from their administrative role. Also removes all of their permissions.<br/>
    /// Params: campaignGuid (Guid), userEmail (string)<br/>
    /// </summary>
    public const string RemoveUserFromAdministrativeRole = "usp_RemoveFromAdministrativeRole";

    /// <summary>
    /// Gets the name of a campaign by its Guid.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetCampaignNameByGuid = "usp_GetCampaignNameByGuid";

    /// <summary>
    /// Adds a new job to be performed for the campaign.<br/>
    /// Params: campaignGuid (Guid), jobName (string), jobDescription (string), jobLocation (string),
    /// jobStartTime (DateTime), jobEndTime (DateTime), jobDefaultSalary (int)<br/>
    /// Output: jobGuid (Guid)<br/>
    /// </summary>
    public const string AddJob = "usp_AddJob";

    /// <summary>
    /// Adds a new job type for the campaign to use.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string), jobTypeDescription (string)<br/>
    /// Throws: Error status code 1 if the job name is already in use.
    /// </summary>
    public const string AddJobType = "usp_AddJobType";

    /// <summary>
    /// Deletes a job type from the campaign.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string)<br/>
    /// </summary>
    public const string DeleteJobType = "usp_DeleteJobType";

    /// <summary>
    /// Updates a job type's name and / or description.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string), jobTypeDescription (string)<br/>
    /// </summary>
    public const string UpdateJobType = "usp_UpdateJobType";

    /// <summary>
    /// Gets the custom and built in job types for a campaign.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetJobTypes = "usp_GetJobTypes";
    
    /// <summary>
    /// Updates a job's name, description, location, start time, end time, and default salary.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), jobName (string), jobDescription (string),
    /// jobLocation (string), jobStartTime (DateTime), jobEndTime (DateTime), jobDefaultSalary (int)<br/>
    /// All parameters but campaignGuid and jobGuid are optional - only the ones that are provided will be updated.
    /// </summary>
    public const string UpdateJob = "usp_UpdateJob";
    
    /// <summary>
    /// Gets a list of all the jobs for a campaign.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetJobs = "usp_GetJobs";
    
    /// <summary>
    /// Gets a single job by its Guid.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid)<br/>
    /// </summary>
    public const string GetJob = "usp_GetJob";
    
    /// <summary>
    /// Deletes a job from the campaign.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid)<br/>
    /// </summary>
    public const string DeleteJob = "usp_DeleteJob";
}
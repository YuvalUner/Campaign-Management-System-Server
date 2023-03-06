namespace DAL.DbAccess;

/// <summary>
/// A collection of all the stored procedure names used in the app.<br/>
/// For the values of the status codes returned by the stored procedures, see <see cref="CustomStatusCode"/>.<br/>
/// Stored procedures that return a status code upon failure return it as the return value of the stored procedure.<br/>
/// Successful stored procedures return 0, which is the Ok status code.
/// </summary>
public static class StoredProcedureNames
{
    
    /// <summary>
    /// Gets a user's info by their email.<br/>
    /// Params: email (string)
    /// </summary>
    public const string GetAllUserInfoByEmail = "usp_UserInfoByEmailGet";

    /// <summary>
    /// Adds a user to the users table.<br/>
    /// Params: email (String), firstNameEng (string), lastNameEng (string), displayNameEng(str), profilePicUrl (str)<br/>
    /// Returns: newly created user's id (int)
    /// </summary>
    public const string CreateUser = "usp_UserAdd";

    /// <summary>
    /// Adds a campaign to the campaigns table.<br/>
    /// Params: campaignName (string), campaignCreatorUserId (int), campaignDescription (string).<br/>
    /// Returns: newly created campaign's id (int)
    /// </summary>
    public const string AddCampaign = "usp_CampaignAdd";
    
    /// <summary>
    /// Links a user to a campaign by adding a row to the campaign_users table.<br/>
    /// Params: campaignGuid (Guid), userId (int)
    /// Returns: Status code DuplicateKey if the user is already in the campaign.
    /// </summary>
    public const string LinkUserToCampaign = "usp_UserLinkToCampaign";

    /// <summary>
    /// Gets public info - name in English and Hebrew and profile picture by a user's user id.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string GetUserPublicInfoByUserId = "usp_UserPublicInfoByIdGet";

    /// <summary>
    /// Gets all campaigns the user is a part of, along with their role in the campaign.<br/>
    /// Params: userId (int).
    /// </summary>
    public const string GetUserCampaigns = "usp_UserCampaignsGet";
    
    /// <summary>
    /// Gets a single row from the voters ledger by the voter's ID number.<br/>
    /// Params: voterId (int)
    /// </summary>
    public const string GetFromVotersLedgerById = "usp_VotersLedgerEntryByIdNumGet";

    /// <summary>
    /// Adds the user's private info to the users and dynamic ledger tables.<br/>
    /// Params: userId (int), firstNameHeb (string), lastNameHeb (string), idNum (int).
    /// Returns: Ok on success, IdAlreadyExistsWhenVerifyingInfo on failure.
    /// </summary>
    public const string AddUserPrivateInfo = "usp_UserPrivateInfoAdd";

    /// <summary>
    /// Gets whether the user is authenticated or not.<br/>
    /// Params: userId (int).
    /// </summary>
    public const string GetUserAuthenticationStatus = "usp_UserAuthenticationStatusGet";

    /// <summary>
    /// Modifies a campaign's info in the database. <br/>
    /// Params: campaignGuid (Guid), campaignDescription (string?), campaignLogoUrl (string?).
    /// </summary>
    public const string ModifyCampaign = "usp_CampaignUpdate";
    
    /// <summary>
    /// Gets the Guid of a campaign by its id.<br/>
    /// Params: campaignId (int)
    /// </summary>
    public const string GetGuidByCampaignId = "usp_CampaignGuidByIdGet";

    /// <summary>
    /// Creates a new invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string CreateCampaignInvite = "usp_CampaignInviteGuidUpdate";
    
    /// <summary>
    /// Revokes an invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string RevokeCampaignInvite = "usp_CampaignInviteGuidDelete";
    
    /// <summary>
    /// Gets the invite GUID for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetCampaignInviteGuid = "usp_CampaignInviteGuidGet";
    
    /// <summary>
    /// Gets the campaign's GUID by its invite GUID.<br/>
    /// Params: campaignInviteGuid (Guid)
    /// </summary>
    public const string GetCampaignByInviteGuid = "usp_CampaignByInviteGuidGet";

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
    public const string FilterVotersLedger = "usp_VotersLedgerRecordsFilter";

    /// <summary>
    /// Gets the type and city of a campaign by its GUID.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetCampaignType = "usp_CampaignTypeAndCityByGuidGet";
    
    /// <summary>
    /// Deletes a user from the database.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string DeleteUser = "usp_UserDelete";
    
    /// <summary>
    /// Adds a permission to a user in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), permissionType (string), permissionForScreen (string).<br/>
    /// Returns: Status Code PermissionDoesNotExist if the permission does not exist,
    /// UserAlreadyHasPermission if the user already has the permission.
    /// </summary>
    public const string AddPermission = "usp_PermissionAdd";

    /// <summary>
    /// Gets the user's permission set for a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).
    /// </summary>
    public const string GetPermissions = "usp_PermissionSetGet";
    
    /// <summary>
    /// Removes a permission from the user in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), permissionType (string), permissionForScreen (string).
    /// </summary>
    public const string RemovePermission = "usp_PermissionRemove";

    /// <summary>
    /// Gets the role a user is assigned to in a campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int).
    /// </summary>
    public const string GetUserRoleInCampaign = "usp_UserRoleGet";
    
    /// <summary>
    /// Gets the list of users in a campaign.<br/>
    /// Params: campaignGuid (Guid).
    /// </summary>
    public const string GetUsersInCampaign = "usp_CampaignUsersGet";

    /// <summary>
    /// Updates a user's support status for a campaign.<br/>
    /// Params: voterId (int), campaignGuid (Guid), supportStatus (bool).
    /// If support status is unknown, pass null.<br/>
    /// Returns: Status code CityNotFound if the city was not found, Ok if the update was successful.
    /// </summary>
    public const string UpdateSupportStatus = "usp_SupportStatusUpdate";
    
    /// <summary>
    /// Deletes a campaign.
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string DeleteCampaign = "usp_CampaignDelete";

    /// <summary>
    /// Adds a phone number to a user, and also updates their record in the voters ledger.<br/>
    /// Params: userId (int), phoneNumber (string)
    /// </summary>
    public const string AddUserPhoneNumber = "usp_UserPhoneNumberAdd";

    /// <summary>
    /// Gets the email and phone number of a user.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string GetUserContactInfo = "usp_UserContactInfoGet";

    /// <summary>
    /// Modifies a user's notification settings for when someone joins a campaign.<br/>
    /// Params: userId (int), campaignGuid (Guid), viaEmail (bool), viaSms (bool)<br/>
    /// To remove the user from the notification list, pass null for both viaEmail and viaSms.
    /// </summary>
    public const string ModifyUserToNotify = "usp_UserToNotifyModify";

    /// <summary>
    /// Adds a verification code for a user's phone number.<br/>
    /// Params: userId (int), phoneNumber (string), verificationCode (string)
    /// </summary>
    public const string AddVerificationCode = "usp_VerificationCodeAdd";

    /// <summary>
    /// Gets a verification code for a user's phone number.<br/>
    /// Params: userId (int)
    /// </summary>
    public const string GetVerificationCode = "usp_VerificationCodeGet";

    /// <summary>
    /// Approves a user's phone number if the server completed its checks.<br/>
    /// Deletes the verification code from the database and adds it to the user.<br/>
    /// Params: userId (int), phoneNumber (string)<br/>
    /// </summary>
    public const string ApproveVerificationCode = "usp_VerificationCodeApprove";

    /// <summary>
    /// Gets a list of users with their contact details and notification settings for a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetUsersToNotify = "usp_UsersToNotifyGet";

    /// <summary>
    /// Gets a list of all the roles in a campaign.<br/>
    /// Params: campaignGuid (Guid)
    /// </summary>
    public const string GetCampaignRoles = "usp_CampaignRolesGet";

    /// <summary>
    /// Adds a custom role to a campaign.<br/>
    /// Params: campaignGuid (Guid), roleName (string), roleDescription (string)<br/>
    /// Returns: Status Code TooManyEntries if campaign already has 50 roles, RoleAlreadyExists if the role already exists.
    /// </summary>
    public const string AddCustomRole = "usp_RoleAdd";

    /// <summary>
    /// Assigns a user to an administrative role and gives them full permissions within the campaign.<br/>
    /// Params: campaignGuid (Guid), userId (int), roleName (string)<br/>
    /// Returns: Status Code DuplicateKeyError if user is already assigned to an administrative role,
    /// UserNotFound if the user was not found, RoleNotFound if the role was not found,
    /// </summary>
    public const string AssignUserToAdministrativeRole = "usp_RoleAdministrativeAssignUser";

    /// <summary>
    /// Assigns a user to a normal, non administrative role.<br/>
    /// Params: campaignGuid (Guid), userId (int), roleName (string)<br/>
    /// Returns: UserNotFound if the user was not found, RoleNotFound if the role was not found,
    /// </summary>
    public const string AssignUserToRole = "usp_RoleAssignUser";

    /// <summary>
    /// Deletes a role from a campaign.<br/>
    /// Params: campaignGuid (Guid), roleName (string)<br/>
    /// </summary>
    public const string DeleteRole = "usp_RoleDelete";

    /// <summary>
    /// Updates a role's description.<br/>
    /// Params: campaignGuid (Guid), roleName (string), roleDescription (string)<br/>
    /// </summary>
    public const string UpdateRole = "usp_RoleUpdate";


    /// <summary>
    /// Removes a user from their role and sets them back to the volunteer role.<br/>
    /// Params: campaignGuid (Guid), userEmail (string)<br/>
    /// </summary>
    public const string RemoveUserFromRole = "usp_RoleRemove";

    /// <summary>
    /// Gets a single role by its name and campaign.<br/>
    /// Params: campaignGuid (Guid), roleName (string)<br/>
    /// </summary>
    public const string GetRole = "usp_RoleGet";

    /// <summary>
    /// Removes a user from their administrative role. Also removes all of their permissions.<br/>
    /// Params: campaignGuid (Guid), userEmail (string)<br/>
    /// </summary>
    public const string RemoveUserFromAdministrativeRole = "usp_RoleAdministrativeRemove";

    /// <summary>
    /// Gets the name of a campaign by its Guid.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetCampaignNameByGuid = "usp_CampaignNameByGuidGet";

    /// <summary>
    /// Adds a new job to be performed for the campaign.<br/>
    /// Params: campaignGuid (Guid), jobName (string), jobDescription (string), jobLocation (string),
    /// jobStartTime (DateTime), jobEndTime (DateTime), jobDefaultSalary (int)<br/>
    /// Output: jobGuid (Guid)<br/>
    /// </summary>
    public const string AddJob = "usp_JobAdd";

    /// <summary>
    /// Adds a new job type for the campaign to use.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string), jobTypeDescription (string)<br/>
    /// Throws: Status code CannotInsertDuplicateUniqueIndex if the job type name is already in use.
    /// </summary>
    public const string AddJobType = "usp_JobTypeAdd";

    /// <summary>
    /// Deletes a job type from the campaign.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string)<br/>
    /// </summary>
    public const string DeleteJobType = "usp_JobTypeDelete";

    /// <summary>
    /// Updates a job type's name and / or description.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string), jobTypeDescription (string)<br/>
    /// Returns: Status code CannotInsertDuplicateUniqueIndex if the job type name is already in use.
    /// </summary>
    public const string UpdateJobType = "usp_JobTypeUpdate";

    /// <summary>
    /// Gets the custom and built in job types for a campaign.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetJobTypes = "usp_JobTypesGet";
    
    /// <summary>
    /// Updates a job's name, description, location, start time, end time, and default salary.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), jobName (string), jobDescription (string),
    /// jobLocation (string), jobStartTime (DateTime), jobEndTime (DateTime), jobDefaultSalary (int)<br/>
    /// All parameters but campaignGuid and jobGuid are optional - only the ones that are provided will be updated.
    /// </summary>
    public const string UpdateJob = "usp_JobUpdate";
    
    /// <summary>
    /// Gets a list of all the jobs for a campaign.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetJobs = "usp_JobsGet";
    
    /// <summary>
    /// Gets a single job by its Guid.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid)<br/>
    /// </summary>
    public const string GetJob = "usp_JobGet";
    
    /// <summary>
    /// Deletes a job from the campaign.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid)<br/>
    /// </summary>
    public const string DeleteJob = "usp_JobDelete";
    
    /// <summary>
    /// Gets a list of jobs for a campaign by whether they are fully manned or not.<br/>
    /// Params: campaignGuid (Guid), fullyManned (bool)<br/>
    /// </summary>
    public const string GetJobsFiltered= "usp_JobsByFilterGet";

    /// <summary>
    /// Adds a new user who can assign other users to a job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), userEmail (string)<br/>
    /// Returns: Status code UserNotFound if the user does not exist, JobNotFound if the job does not exist,
    /// DuplicateKey if the user is already capable of assigning to the job.
    /// </summary>
    public const string AddUserWhoCanAssignToJob = "usp_JobAssignCapableUsersAdd";
    
    /// <summary>
    /// Adds a new user who can assign other users to any job with a specific job type.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string), userEmail (string)<br/>
    /// Returns: Status code UserNotFound if the user does not exist, JobTypeNotFound if the job type does not exist,
    /// DuplicateKey if the user is already capable of assigning to the job type.
    /// </summary>
    public const string AddUserWhoCanAssignToJobType = "usp_JobTypeAssignCapableUserAdd";
    
    /// <summary>
    /// Removes a user from being capable of assigning other users to a job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), userEmail (string)<br/>
    /// </summary>
    public const string RemoveUserWhoCanAssignToJob = "usp_JobAssignCapableUsersRemove";
    
    /// <summary>
    /// Removes a user from being capable of assigning other users to any job with a specific job type.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string), userEmail (string)<br/>
    /// </summary>
    public const string RemoveUserWhoCanAssignToJobType = "usp_JobTypeAssignCapableUsersRemove";
    
    /// <summary>
    /// Gets the list of users who can assign other users to a job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), viaJobType (bool) - optional.<br/>
    /// Setting viaJobType to true will return the list of anyone who can assign to that job, regardless of whether
    /// it is job specific or job type specific.<br/>
    /// Setting it to false will return the list of anyone who can assign to that job specifically, and not via job type.<br/>
    /// </summary>
    public const string GetUsersWhoCanAssignToJob = "usp_JobAssignCapableUsersGet";
    
    /// <summary>
    /// Gets the list of users who can assign other users to any job with a specific job type.<br/>
    /// Params: campaignGuid (Guid), jobTypeName (string)<br/>
    /// </summary>
    public const string GetUsersWhoCanAssignToJobType = "usp_JobTypeAssignCapableUsersGet";
    
    /// <summary>
    /// Assigns a user to a job, and increments the number of people assigned to that job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), userEmail (string), salary (int) optional.<br/>
    /// Returns: Status code UserNotFound if the user does not exist, JobNotFound if the job does not exist,
    /// JobFullyManned if the job is already fully manned, DuplicateKey if user is already assigned to job.<br/>
    /// </summary>
    public const string AssignToJob = "usp_JobAssign";
    
    /// <summary>
    /// Removes a user from being assigned to a job, and decrements the number of people assigned to that job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), userEmail (string)<br/>
    /// Returns: Status code UserNotFound if the user does not exist, JobNotFound if the job does not exist.
    /// </summary>
    public const string RemoveJobAssignment = "usp_JobAssignmentRemove";
    
    /// <summary>
    /// Gets the list of users who are assigned to a job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid)<br/>
    /// </summary>
    public const string GetUsersAssignedToJob = "usp_JobAssignedUsersGet";
    
    /// <summary>
    /// Updates the salary of a user assigned to a job.<br/>
    /// Params: campaignGuid (Guid), jobGuid (Guid), userEmail (string), salary (int)<br/>
    /// Returns: Status code UserNotFound if the user does not exist, JobNotFound if the job does not exist.
    /// </summary>
    public const string UpdateJobAssignment = "usp_JobAssignmentUpdate";

    /// <summary>
    /// Gets the list of jobs that a user is assigned to.<br/>
    /// Params: userId (int), campaignGuid (Guid) - optional.<br/>
    /// Leaving campaignGuid blank will return all jobs for all campaigns that the user is assigned to, as well as
    /// the campaign names and guids of those campaigns.<br/>
    /// </summary>
    public const string GetUserJobs = "usp_UserJobsGet";
    
    /// <summary>
    /// Modifies the user's preferences for the jobs and conditions they prefer.<br/>
    /// If the user has no preferences yet, they will be added.<br/>
    /// If the user has preferences already, they will be updated.<br/>
    /// Params: userId (int), campaignGuid (Guid), UserPreferencesText (string)<br/>
    /// If UserPreferencesText is null, the user's preferences will be deleted.<br/>
    /// </summary>
    public const string ModifyUserJobPreferences = "usp_UserPreferencesModify";
    
    /// <summary>
    /// Gets the user's preferences for the jobs and conditions they prefer.<br/>
    /// Params: userId (int), campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetUserJobPreferences = "usp_UserPreferencesGet";

    /// <summary>
    /// Gets the basic info of a campaign - name, description, guid, type, creation date, city and logo url.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetCampaignBasicInfo = "usp_CampaignBasicInfoGet";

    /// <summary>
    /// Removes a user's phone number from their account.<br/>
    /// Params: userId (int)<br/>
    /// </summary>
    public const string RemoveUserPhoneNumber = "usp_UserPhoneNumberRemove";
    
    /// <summary>
    /// Gets a list of users for a campaign, based on various filters.<br/>
    /// Params: campaignGuid (Guid), firstName (string) - optional, lastName (string) - optional, email (string) - optional,
    /// idNum (int) - optional, phoneNum (string) - optional, city (string) - optional, street (string) - optional,
    /// jobStartTime (DateTime) - optional, jobEndTime (DateTime) - optional<br/>
    /// jobStartTime and jobEndTime are used to get the list of users who are available to work during that time.<br/>
    /// </summary>
    public const string FilterUsersList = "usp_UsersListFilter";

    /// <summary>
    /// Adds a new sms message to the database.<br/>
    /// Params: campaignGuid (Guid), messageContent (string), senderId (int)., newMessageGuid (Guid) - output,
    /// newMessageId (int) - output.<br/>
    /// </summary>
    public const string AddSmsMessage = "usp_SmsMessageAdd";
    
    /// <summary>
    /// Records the sending of a message to a phone number.<br/>
    /// Params: messageId (int), phoneNumber (string), isSuccess (bool) <br/>
    /// </summary>
    public const string AddSmsMessageSent = "usp_SmsMessageSentToPhoneNumberAdd";

    /// <summary>
    /// Gets the sms logs of a specific campaign.<br/>
    /// Only gets the basic info, without going into the details of each message - those are in a different SP.<br/>
    /// Params: campaignGuid (Guid)<br/>
    /// </summary>
    public const string GetBaseSmsLogs = "usp_SmsMessageGeneralLogsGet";
    
    /// <summary>
    /// Gets the log for a specific sms message.<br/>
    /// Returns the phone numbers it was sent to, and whether it was successfully sent to each phone number.<br/>
    /// In addition, if the phone number can be associated with someone, their name and address will be returned.<br/>
    /// </summary>
    public const string GetSmsDetailsLog = "usp_SmsInDepthLogDetailsGet";
    
    /// <summary>
    /// Adds a new event to the database.<br/>
    /// Params: eventName (string), eventDescription (string) - optional, eventStartTime (DateTime) - optional,
    /// eventEndTime (DateTime) - optional, maxAttendees (int) - optional, eventLocation (string) - optional,
    /// userId (int), campaignGuid (Guid) - optional, newEventGuid (Guid) - output, newEventId (int) - output.<br/>
    /// Returns: Status code CampaignNotFound if campaign was provided but not found.<br/>
    /// </summary>
    public const string AddEvent = "usp_EventAdd";
    
    /// <summary>
    /// Updates an existing event in the database.<br/>
    /// Params: eventGuid (Guid), eventName (string) - optional, eventDescription (string) - optional,
    /// eventLocation (string) - optional, eventStartTime (DateTime) - optional, eventEndTime (DateTime) - optional,
    /// maxAttendees (int) - optional, campaignGuid (Guid) - optional.<br/>
    /// Returns: Status code EventNotFound if the event does not exist, Status code CampaignNotFound if campaign was
    /// provided but not found.<br/>
    /// </summary>
    public const string UpdateEvent = "usp_EventUpdate";
    
    /// <summary>
    /// Deletes an existing event from the database.<br/>
    /// Params: eventGuid (Guid)<br/>
    /// Returns: Status code EventNotFound if the event does not exist.<br/>
    /// </summary>
    public const string DeleteEvent = "usp_EventDelete";

    /// <summary>
    /// Assigns a user to an event as a participant. Removes the user from being a watcher if they already are one.<br/>
    /// Params: eventGuid (Guid), userId (int) - optional, userEmail (string) - optional.<br/>
    /// Returns: EventNotFound if the event does not exist, UserNotFound if the user does not exist,
    /// ParameterMustNotBeNullOrEmpty if both userId and userEmail are empty, TooManyValuesProvided if both
    /// userId and userEmail are provided, DuplicateKey if user is already assigned to the event.<br/>
    /// </summary>
    public const string AssignToEvent = "usp_EventAssignTo";
    
    /// <summary>
    /// Gets all events a user is assigned to.<br/>
    /// Params: userId (int).<br/>
    /// </summary>
    public const string GetUserEvents = "usp_EventsGetForUser";
    
    /// <summary>
    /// Adds a watcher to an event. Unlike participants, watchers do not count towards the event attendance. 
    /// Removes the user's participation in the event if they are already participants.<br/>
    /// Params: eventGuid (Guid), userId (int).<br/>
    /// Returns: EventNotFound if the event does not exist, DuplicateKey if user is already a watcher of that event.<br/>
    /// </summary>
    public const string AddWatcherToEvent = "usp_EventAddWatcher";
    
    /// <summary>
    /// Removes a participant from an event, and updates the event's attendance count.<br/>
    /// Params: eventGuid (Guid), userId (int) - optional, userEmail (string) - optional.<br/>
    /// Returns: EventNotFound if the event does not exist, UserNotFound if the user does not exist,
    /// ParameterMustNotBeNullOrEmpty if both userId and userEmail are empty, TooManyValuesProvided if both
    /// userId and userEmail are provided.<br/>
    /// </summary>
    public const string RemoveEventParticipant = "usp_EventParticipationDelete";
    
    /// <summary>
    /// Removes a watcher from an event.<br/>
    /// Params: eventGuid (Guid), userId (int).<br/>
    /// Returns: EventNotFound if the event does not exist.<br/>
    /// </summary>
    public const string RemoveEventWatcher = "usp_EventWatcherDelete";
    
    /// <summary>
    /// Gets all events related to a campaign.<br/>
    /// Params: campaignGuid (Guid).<br/>
    /// </summary>
    public const string GetCampaignEvents = "usp_EventsGetForCampaign";
    
    /// <summary>
    /// Gets all participants of a specific event.<br/>
    /// Params: eventGuid (Guid).<br/>
    /// Returns: EventNotFound if the event does not exist.<br/>
    /// </summary>
    public const string GetEventParticipants = "usp_EventGetParticipants";
    
    /// <summary>
    /// Gets the details of a specific event.<br/>
    /// Params: eventGuid (Guid).<br/>
    /// </summary>
    public const string GetEvent = "usp_EventGet";
    
    /// <summary>
    /// Gets the user id of the creator of a specific event.<br/>
    /// Params: eventGuid (Guid).<br/>
    /// </summary>
    public const string GetEventCreatorUserId = "usp_EventsGetCreatorUserId";
    
    /// <summary>
    /// Gets all users who can manage another specific user's schedule.<br/>
    /// Params: userEmail (string) - optional, userId (int) - optional.<br/>
    /// Returns: ParameterMustNotBeNullOrEmpty if both userId and userEmail are empty, TooManyValuesProvided if both
    /// userId and userEmail are provided.<br/>
    /// </summary>
    public const string GetScheduleManagers = "usp_EventScheduleManagersGet";

    /// <summary>
    /// Adds a new schedule manager to a user.<br/>
    /// Params: giverUserId (int), receiverEmail (string).<br/>
    /// Returns: Status code UserNotFound if the permission receiver does not exist,
    /// DuplicateKey if user is already a schedule manage of the requesting user.<br/>
    /// </summary>
    public const string AddScheduleManager = "usp_EventScheduleManagerAdd";

    /// <summary>
    /// Removes a schedule manager from a user.<br/>
    /// Params: giverUserId (int), receiverEmail (string).<br/>
    /// Returns: Status code UserNotFound if the permission receiver does not exist.<br/>
    /// </summary>
    public const string RemoveScheduleManager = "usp_EventScheduleManagerRemove";
    
    /// <summary>
    /// Gets all users who can be managed by a specific user.<br/>
    /// Params: userId (int).<br/>
    /// </summary>
    public const string GetManagedUsers = "usp_EventScheduleManagedUsersGet";
    
    /// <summary>
    /// Gets a list of all the user's personal events, those created by them or for them.<br/>
    /// Params: userId (int).<br/>
    /// </summary>
    public const string GetPersonalEvents = "usp_EventsGetPersonal";

    /// <summary>
    /// Gets all watchers of a specific event.<br/>
    /// Params: eventGuid (Guid).<br/>
    /// </summary>
    public const string GetEventWatchers = "usp_EventGetWatchers";

    /// <summary>
    /// Publishes an event to the public board by adding it to the relevant table.<br/>
    /// Params: eventGuid (Guid), publisherId (int).<br/>
    /// Returns: Status code EventNotFound if the event does not exist, DuplicateKey if the event is already published,
    /// IncorrectEventType if event is not associated to any campaign, UserNotFound if publisher id is not a valid user.<br/>
    /// </summary>
    public const string PublishEvent = "usp_PublicBoardEventAdd";
    
    /// <summary>
    /// Unpublishes an event from the public board by removing it from the relevant table.<br/>
    /// Params: eventGuid (Guid).<br/>
    /// Returns: Status code EventNotFound if the event does not exist.<br/>
    /// </summary>
    public const string UnpublishEvent = "usp_PublicBoardEventDelete";

    /// <summary>
    /// Gets all published events for a specific campaign. Also gets details about the publisher of the events.<br/>
    /// Params: campaignGuid (Guid).<br/>
    /// Returns: Status code CampaignNotFound if the campaign does not exist.<br/>
    /// </summary>
    public const string GetCampaignPublishedEvents = "usp_PublicBoardEventsGetForCampaign";
    
    /// <summary>
    /// Adds a new announcement to the public board by adding it to the relevant table.<br/>
    /// Params: campaignGuid (Guid), publisherId (int), announcementTitle (string), announcementContent (string),
    /// newAnnouncementGuid (Guid) - output.<br/>
    /// Returns: Status code CampaignNotFound if the campaign does not exist, UserNotFound if publisher id is not a valid user.<br/>
    /// </summary>
    public const string PublishAnnouncement = "usp_PublicBoardAnnouncementAdd";
    
    /// <summary>
    /// Unpublishes an announcement from the public board by removing it from the relevant table.<br/>
    /// Params: announcementGuid (Guid).<br/>
    /// Returns: Status code AnnouncementNotFound if the announcement does not exist.<br/>
    /// </summary>
    public const string UnpublishAnnouncement = "usp_PublicBoardAnnouncementDelete";
    
    /// <summary>
    /// Gets all published announcements for a specific campaign. Also gets details about the publisher of the announcements.<br/>
    /// Params: campaignGuid (Guid).<br/>
    /// Returns: Status code CampaignNotFound if the campaign does not exist.<br/>
    /// </summary>
    public const string GetCampaignPublishedAnnouncements = "usp_PublicBoardAnnouncementsGetForCampaign";
    
    /// <summary>
    /// Adds a new user preference to the database.<br/>
    /// Params: userId (int), campaignGuid (Guid), isPreferred (bool) - set to true if the user wants to prioritize updates
    /// from this campaign, and false if the user wants to avoid updates from this campaign.<br/>
    /// Returns: Status code UserNotFound if the user does not exist, CampaignNotFound if the campaign does not exist,
    /// DuplicateKey if the user already has a preference for this campaign.<br/>
    /// </summary>
    public const string AddUserPreference = "usp_UserPreferenceAdd";
    
    /// <summary>
    /// Removes a user preference from the database.<br/>
    /// Params: userId (int), campaignGuid (Guid).<br/>
    /// Returns: Status code UserNotFound if the user does not exist, CampaignNotFound if the campaign does not exist,
    /// PreferenceNotFound if the user does not have a preference for this campaign.<br/>
    /// </summary>
    public const string DeleteUserPreference = "usp_UserPreferenceDelete";
    
    /// <summary>
    /// Updates a user preference in the database.<br/>
    /// Params: userId (int), campaignGuid (Guid), isPreferred (bool) - set to true if the user wants to prioritize updates, 
    /// and false if the user wants to avoid updates.<br/>
    /// Returns: Status code UserNotFound if the user does not exist, CampaignNotFound if the campaign does not exist,
    /// PreferenceNotFound if the user does not have a preference for this campaign.<br/>
    /// </summary>
    public const string UpdateUserPreference = "usp_UserPreferenceUpdate";
    
    /// <summary>
    /// Gets the list of user preferences from the database, including the campaign name, logo and campaign guid.<br/>
    /// Params: userId (int).<br/>
    /// Returns: Status code UserNotFound if the user does not exist.<br/>
    /// </summary>
    public const string GetUserPreferences = "usp_UserPreferenceGet";
    
    /// <summary>
    /// Gets published events for a specific user, based on their preferences.<br/>
    /// Events are ordered such that events from preferred campaigns are returned first, followed by events from
    /// other campaigns, and all are ordered by publishing date.<br/>
    /// Events from avoided campaigns are filtered out.<br/>
    /// Params: userId (int), limit (int) - optional, how many rows to return, defaults to 50.<br/>
    /// </summary>
    public const string GetPublishedEventsByUserPreferences = "usp_PublicBoardEventsGetForUserByPreferences";
    
    /// <summary>
    /// Gets published announcements for a specific user, based on their preferences.<br/>
    /// Announcements are ordered such that announcements from preferred campaigns are returned first, followed by
    /// other campaigns, and all are ordered by publishing date.<br/>
    /// Announcements from avoided campaigns are filtered out.<br/>
    /// Params: userId (int), limit (int) - optional, how many rows to return, defaults to 50.<br/>
    /// </summary>
    public const string GetPublishedAnnouncementsByUserPreferences = "usp_PublicBoardAnnouncementsGetForUserByPreferences";
    
    /// <summary>
    /// Searches for published events by the given parameters.<br/>
    /// Params: @campaignGuid uniqueidentifier, @campaignName nvarchar(200), @campaignCity nvarchar(100),
    /// @publishingDate datetime, @eventName nvarchar(200), @publisherFirstName nvarchar(200), @publisherLastName nvarchar(200),
    /// @eventLocation nvarchar(100), @eventStartTime datetime, @eventEndTime datetime <br/>
    /// All parameters are optional, and if they are not provided, they are ignored.<br/>
    /// The parameters are written like this because I could not be bothered to write them one by one, instead I just
    /// copied the parameters from the stored procedure.<br/>
    /// </summary>
    public const string SearchPublishedEvents = "usp_PublicBoardEventsSearch";
    
    /// <summary>
    /// Searches for published announcements by the given parameters.<br/>
    /// Params: @campaignGuid uniqueidentifier, @campaignName nvarchar(200), @campaignCity nvarchar(100),
    /// @publishingDate datetime, @announcementTitle nvarchar(100), @publisherFirstName nvarchar(200),
    /// @publisherLastName nvarchar(200)<br/>
    /// All parameters are optional, and if they are not provided, they are ignored.<br/>
    /// The parameters are written like this because I could not be bothered to write them one by one, instead I just
    /// copied the parameters from the stored procedure.<br/>
    /// </summary>
    public const string SearchPublishedAnnouncements = "usp_PublicBoardAnnouncementsSearch";
}
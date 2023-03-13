using DAL.Models;

namespace API.Utils;

/// <summary>
/// A collection of constants used throughout the API.<br/>
/// Mostly used when getting values from the session.<br/>
/// </summary>
public static class Constants
{
    /// <summary>
    /// The key used to get the user id from the session.<br/>
    /// The user id is an integer that is used to identify the user, and is stored in the session once the user logs in.<br/>
    /// Use Session.GetInt32(Constants.UserId) to get the user id.<br/>
    /// </summary>
    public const string UserId = "userID";

    /// <summary>
    /// The authentication status of the user - whether they have authenticated their details or not.<br/>
    /// A boolean value that is stored in the session once the user logs in.<br/>
    /// Use Session.Get&lt;bool&gt;(Constants.UserAuthenticationStatus) to get the authentication status.<br/>
    /// </summary>
    public const string UserAuthenticationStatus = "authenticationStatus";

    /// <summary>
    /// A list of campaigns the user is allowed to access.<br/>
    /// That is, the campaigns the user is a member of.<br/>
    /// A list of <see cref="Campaign"/> objects that is stored in the session once the user logs in.<br/>
    /// Use Session.Get&lt;List&lt;Campaign&gt;&gt;(Constants.AllowedCampaigns) to get the list of campaigns.<br/>
    /// </summary>
    public const string AllowedCampaigns = "allowedCampaigns";

    /// <summary>
    /// The campaign the user is currently active in.<br/>
    /// Activity in any campaign except the active one is not allowed.<br/>
    /// It is a <see cref="Campaign"/> object that is stored in the session once the user logs in.<br/>
    /// Use Session.Get&lt;Campaign&gt;(Constants.ActiveCampaign) to get the active campaign.<br/>
    /// </summary>
    public const string ActiveCampaign = "activeCampaign";

    /// <summary>
    /// A list of permissions the user has in their active campaign.<br/>
    /// A list of <see cref="Permission"/> objects that is stored in the session once the user enters a campaign.<br/>
    /// Use Session.Get&lt;List&lt;Permission&gt;&gt;(Constants.Permissions) to get the list of permissions.<br/>
    /// </summary>
    public const string Permissions = "permissions";

    /// <summary>
    /// The role the user has in their active campaign.<br/>
    /// A <see cref="Role"/> object that is stored in the session once the user enters a campaign.<br/>
    /// Use Session.Get&lt;Role&gt;(Constants.ActiveCampaignRole) to get the role.<br/>
    /// </summary>
    public const string ActiveCampaignRole = "activeCampaignRole";
}
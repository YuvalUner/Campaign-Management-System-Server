using API.SessionExtensions;
using DAL.Models;
using DAL.Services.Interfaces;

namespace API.Utils;

/// <summary>
/// Was meant to be a collection of utility methods for job assignment, but is currently only used for one method.<br/>
/// The method, <see cref="CanModifyJobAssignment"/>, checks if the user can modify the job assignment.<br/>
/// </summary>
public static class JobAssignmentUtils
{
    /// <summary>
    /// Checks if the user can modify the job assignment, according to their permissions and role.<br/>
    /// </summary>
    /// <param name="jobAssignmentCapabilityService">An implementation of <see cref="IJobAssignmentCapabilityService"/></param>
    /// <param name="context">The HttpContext object of the controller.</param>
    /// <param name="jobGuid">The Guid of the job that the user wishes to modify assignment for.</param>
    /// <param name="campaignGuid">The Guid of the campaign the job belongs to.</param>
    /// <returns>True if the user can modify the assignment, false otherwise.</returns>
    public static async Task<bool> CanModifyJobAssignment(IJobAssignmentCapabilityService jobAssignmentCapabilityService,
        HttpContext context, Guid jobGuid, Guid campaignGuid)
    {
        var role = context.Session.Get<Role?>(Constants.ActiveCampaignRole);
        if (role == null)
        {
            return false;
        }
        
        // Admin roles can assign to any job
        if (role.RoleLevel > 0)
        {
            return true;
        }
        
        // If the user is not an admin, they can only assign to jobs they have permission to assign to
        var assignmentCapableUsers = await jobAssignmentCapabilityService.
            GetJobAssignmentCapableUsers(campaignGuid, jobGuid, true);
        var userId = context.Session.GetInt32(Constants.UserId);
        
        return assignmentCapableUsers.Any(x => x.UserId == userId);
    }
}
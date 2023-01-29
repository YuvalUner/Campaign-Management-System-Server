using API.SessionExtensions;
using DAL.Models;
using DAL.Services.Interfaces;

namespace API.Utils;

public static class JobAssignmentUtils
{
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
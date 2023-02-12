using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class RolesAndPermissionsServicesTests
{
    private readonly IConfiguration _configuration;
    private readonly IRolesService _rolesService;
    private readonly IPermissionsService _permissionsService;
    
    // User that created the campaign, should have all permissions and owner role
    private static User testUser = new User()
    {
        UserId = 2,
        Email = "aaa",
    };
    
    private static Campaign testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6")
    };
    
    private static User OtherUser = new User()
    {
        UserId = 53,
        Email = "bbb",
    };
    
    private static Role testRole = new Role()
    {
        RoleName = "בדיקה",
        RoleDescription = "TestRoleDescription",
        RoleLevel = 0
    };
    
    private static Permission testPermission = new Permission()
    {
        PermissionTarget = PermissionTargets.VotersLedger,
        PermissionType = PermissionTypes.View
    };

    private static Role ownerRole = new Role()
    {
        RoleName = BuiltInRoleNames.Owner,
        RoleLevel = 3
    };
    
    private static Role volunteerRole = new Role()
    {
        RoleName = BuiltInRoleNames.Volunteer,
        RoleLevel = 0
    };

    private static Role managerRole = new Role()
    {
        RoleName = BuiltInRoleNames.Manager,
        RoleLevel = 1
    };
    
    public RolesAndPermissionsServicesTests()
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build(); 
        _rolesService = new RolesService(new GenericDbAccess(_configuration));
        _permissionsService = new PermissionsService(new GenericDbAccess(_configuration));
    }
    
    [Fact, TestPriority(0)]
    public void GetRolesShouldReturnNoCustomRoles()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRolesInCampaign(testCampaign.CampaignGuid).Result;
        
        // Assert
        // Built in roles should be present
        Assert.NotEmpty(result);
        Assert.DoesNotContain(result, role => role.RoleName == testRole.RoleName);
    }
    
    [Fact, TestPriority(0)]
    public void GetPermissionsShouldReturnEmpty()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.GetPermissions(OtherUser.UserId, testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(1)]
    public void AddPermissionShouldWork()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.AddPermission(testPermission, OtherUser.UserId, testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(2)]
    public void AddPermissionShouldFailForUserAlreadyHasPermission()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.AddPermission(testPermission, OtherUser.UserId, testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserAlreadyHasPermission, result);
    }

    [Fact, TestPriority(2)]
    public void CreateWrongPermissionShouldThrowErrorForWrongTarget()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(() => new Permission()
        {
            PermissionTarget = "WrongTarget",
            PermissionType = PermissionTypes.View
        });
    }
    
    [Fact, TestPriority(2)]
    public void CreateWrongPermissionShouldThrowErrorForWrongType()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(() => new Permission()
        {
            PermissionType = "WrongType",
            PermissionTarget = PermissionTargets.VotersLedger
        });
    }
    
    [Fact, TestPriority(2)]
    public void GetPermissionsShouldReturnOnePermission()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.GetPermissions(OtherUser.UserId, testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Equal(testPermission.PermissionTarget, result.First().PermissionTarget);
        Assert.Equal(testPermission.PermissionType, result.First().PermissionType);
    }
    
    [Fact, TestPriority(2)]
    public void GetPermissionsShouldReturnEmptyForWrongUser()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.GetPermissions(-1, testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(2)]
    public void GetPermissionsShouldReturnEmptyForWrongCampaign()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.GetPermissions(OtherUser.UserId, Guid.Empty).Result;
        
        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(2)]
    public void GetPermissionsShouldReturnAllForAdminUser()
    {
        // Arrange
        
        // Act
        var result = _permissionsService.GetPermissions(testUser.UserId, testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.NotEmpty(result);
        // Check to see that a permission not added here is present
        Assert.Contains(result, permission => permission.PermissionTarget == PermissionTargets.CampaignRoles &&
                                              permission.PermissionType == PermissionTypes.View);
    }
    
    [Fact, TestPriority(3)]
    public void AddRoleShouldWork()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AddRoleToCampaign(testCampaign.CampaignGuid, testRole.RoleName, testRole.RoleDescription).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }
    
    [Fact, TestPriority(4)]
    public void AddRoleShouldFailForRoleAlreadyExists()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AddRoleToCampaign(testCampaign.CampaignGuid, testRole.RoleName, testRole.RoleDescription).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.RoleAlreadyExists, result);
    }
    
    [Fact, TestPriority(4)]
    public void GetRolesShouldReturnOneCustomRole()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRolesInCampaign(testCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, role => role.RoleName == testRole.RoleName 
                                        && role.RoleDescription == testRole.RoleDescription
                                        && role.RoleLevel == testRole.RoleLevel);
    }
    
    [Fact, TestPriority(4)]
    public void GetRolesShouldReturnEmptyForWrongCampaign()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRolesInCampaign(Guid.Empty).Result;
        
        // Assert
        Assert.DoesNotContain(result, role => role.RoleName == testRole.RoleName);
    }

    [Fact, TestPriority(4)]
    public void GetUserRoleForNormalUserShouldBeVolunteer()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRoleInCampaign(testCampaign.CampaignGuid, OtherUser.UserId).Result;
        
        // Assert
        Assert.Equal(volunteerRole.RoleName, result.RoleName);
        Assert.Equal(volunteerRole.RoleLevel, result.RoleLevel);
    }
    
    [Fact, TestPriority(4)]
    public void GetUserRoleForAdminUserShouldBeOwner()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRoleInCampaign(testCampaign.CampaignGuid, testUser.UserId).Result;
        
        // Assert
        Assert.Equal(ownerRole.RoleName, result.RoleName);
        Assert.Equal(ownerRole.RoleLevel, result.RoleLevel);
    }

    [Fact, TestPriority(5)]
    public void AddNormalRoleToUserShouldWork()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToNormalRole(testCampaign.CampaignGuid, OtherUser.Email, testRole.RoleName).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(6)]
    public void AddNormalRoleToUserShouldFailForRoleNotFound()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToNormalRole(testCampaign.CampaignGuid, OtherUser.Email, "WrongRole").Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.RoleNotFound, result);
    }
    
    [Fact, TestPriority(6)]
    public void AddNormalRoleToUserShouldFailForUserNotFound()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToNormalRole(testCampaign.CampaignGuid, "WrongUser", testRole.RoleName).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }
    
    [Fact, TestPriority(6)]
    public void GetUserRoleForNormalUserShouldBeCustomRole()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRoleInCampaign(testCampaign.CampaignGuid, OtherUser.UserId).Result;
        
        // Assert
        Assert.Equal(testRole.RoleName, result.RoleName);
        Assert.Equal(testRole.RoleLevel, result.RoleLevel);
    }

    [Fact, TestPriority(6)]
    public void UpdateRoleShouldWork()
    {
        // Arrange
        testRole.RoleDescription = "New Description";
        
        // Act
        _rolesService.UpdateRole(testCampaign.CampaignGuid, testRole.RoleName, testRole.RoleDescription).Wait();
        
        // Assert
        var result = _rolesService.GetRole(testRole.RoleName, testCampaign.CampaignGuid).Result;
        Assert.Equal(testRole.RoleDescription, result.RoleDescription);
    }

    [Fact, TestPriority(7)]
    public void CheckUserRoleDescriptionChangedShouldWork()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRoleInCampaign(testCampaign.CampaignGuid, OtherUser.UserId).Result;
        
        // Assert
        Assert.Equal(testRole.RoleDescription, result.RoleDescription);
    }
    
    [Fact, TestPriority(8)]
    public void AddAdminRoleToUserShouldWork()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToAdministrativeRole(testCampaign.CampaignGuid, OtherUser.Email, managerRole.RoleName).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(9)]
    public void AddAdminRoleToUserShouldFailForDuplicateKey()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToAdministrativeRole(testCampaign.CampaignGuid, OtherUser.Email, managerRole.RoleName).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, result);
    }
    
    [Fact, TestPriority(9)]
    public void GetUserRoleForNormalUserShouldBeManager()
    {
        // Arrange
        
        // Act
        var result = _rolesService.GetRoleInCampaign(testCampaign.CampaignGuid, OtherUser.UserId).Result;
        
        // Assert
        Assert.Equal(managerRole.RoleName, result.RoleName);
        Assert.Equal(managerRole.RoleLevel, result.RoleLevel);
    }
    
    [Fact, TestPriority(9)]
    public void AddAdminRoleToUserShouldFailForRoleNotFound()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToAdministrativeRole(testCampaign.CampaignGuid, OtherUser.Email, "WrongRole").Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.RoleNotFound, result);
    }
    
    [Fact, TestPriority(9)]
    public void AddAdminRoleToUserShouldFailForUserNotFound()
    {
        // Arrange
        
        // Act
        var result = _rolesService.AssignUserToAdministrativeRole(testCampaign.CampaignGuid, "WrongUser", managerRole.RoleName).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }
    
    [Fact, TestPriority(100)]
    public void RemovePermissionShouldWork()
    {
        // Arrange
        
        // Act
        _permissionsService.RemovePermission(testPermission, OtherUser.UserId, testCampaign.CampaignGuid).Wait();
        
        // Assert
        var result = _permissionsService.GetPermissions(OtherUser.UserId, testCampaign.CampaignGuid).Result;
        Assert.DoesNotContain(result, permission => permission.PermissionTarget == testPermission.PermissionTarget
                                            && permission.PermissionType == testPermission.PermissionType);
    }

    [Fact, TestPriority(101)]
    public void DeleteRoleShouldWork()
    {
        // Arrange
        
        // Act
        _rolesService.DeleteRole(testCampaign.CampaignGuid, testRole.RoleName).Wait();
        
        // Assert
        var result = _rolesService.GetRolesInCampaign(testCampaign.CampaignGuid).Result;
        Assert.DoesNotContain(result, role => role.RoleName == testRole.RoleName);
    }

    [Fact, TestPriority(101)]
    public void RemoveUserFromAdminRoleShouldWork()
    {
        // Arrange
        
        // Act
        _rolesService.RemoveUserFromAdministrativeRole(testCampaign.CampaignGuid, OtherUser.Email).Wait();
        
        // Assert
        var role = _rolesService.GetRoleInCampaign(testCampaign.CampaignGuid, OtherUser.UserId).Result;
        Assert.Equal(volunteerRole.RoleName, role.RoleName);
        Assert.Equal(volunteerRole.RoleLevel, role.RoleLevel);
        
        var permissions = _permissionsService.GetPermissions(OtherUser.UserId, testCampaign.CampaignGuid).Result;
        Assert.Empty(permissions);
    }
}
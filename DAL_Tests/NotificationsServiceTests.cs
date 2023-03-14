using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class NotificationsServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly INotificationsService _notificationsService;

    private static User testUser = new User()
    {
        UserId = 53,
        Email = "bbb",
    };
    
    private static Campaign testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6")
    };
    
    private static NotificationSettings testNotificationSettings = new NotificationSettings()
    {
        ViaSms = true,
        ViaEmail = true,
    };
    
    public NotificationsServiceTests()
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build(); 
        _notificationsService = new NotificationsService(new GenericDbAccess(_configuration));
    }
    
    [Fact, TestPriority(0)]
    public void GetUsersToNotifyShouldReturnEmpty()
    {
        // Arrange
        
        // Act
        var result = _notificationsService.GetUsersToNotify(testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(1)]
    public void AddUserNotificationSettingsShouldWork()
    {
        // Arrange
        
        // Act
        _notificationsService.AddUserToNotify(testUser.UserId, testCampaign.CampaignGuid.Value, testNotificationSettings.ViaSms, testNotificationSettings.ViaEmail).Wait();
        
        // Assert
        var result = _notificationsService.GetUsersToNotify(testCampaign.CampaignGuid.Value).Result;
        Assert.Single(result);
        Assert.Contains(result, x => x.ViaEmail == testNotificationSettings.ViaEmail && x.ViaSms == testNotificationSettings.ViaSms);
    }
    
    [Fact, TestPriority(2)]
    public void UpdateUserNotificationSettingsShouldWork()
    {
        // Arrange
        testNotificationSettings.ViaEmail = true;
        testNotificationSettings.ViaSms = false;
        
        // Act
        _notificationsService.UpdateUserToNotify(testUser.UserId, testCampaign.CampaignGuid.Value, testNotificationSettings.ViaSms, testNotificationSettings.ViaEmail).Wait();
        
        // Assert
        var result = _notificationsService.GetUsersToNotify(testCampaign.CampaignGuid.Value).Result;
        Assert.Single(result);
        Assert.Contains(result, x => x.ViaEmail == testNotificationSettings.ViaEmail && x.ViaSms == testNotificationSettings.ViaSms);
    }
    
    [Fact, TestPriority(3)]
    public void DeleteUserNotificationSettingsShouldWork()
    {
        // Arrange
        
        // Act
        _notificationsService.RemoveUserToNotify(testUser.UserId, testCampaign.CampaignGuid.Value).Wait();
        
        // Assert
        var result = _notificationsService.GetUsersToNotify(testCampaign.CampaignGuid.Value).Result;
        Assert.Empty(result);
    }
}
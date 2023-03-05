using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class UsersPublicBoardPreferenceServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IUsersPublicBoardPreferenceService _usersPublicBoardPreferenceService;
    
    private static User _user = new()
    {
        UserId = 53,
        Email = "bbb"
    };

    private static Campaign _campaign = new()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6"),
        CampaignName = "Test Campaign"
    };
    
    public UsersPublicBoardPreferenceServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        _usersPublicBoardPreferenceService = new UsersPublicBoardPreferenceService(new GenericDbAccess(_configuration));
    }
    
    [Fact, TestPriority(1)]
    public void GetPreferences_ShouldReturnEmptyList_WhenNoPreferences()
    {
        // Arrange
        
        // Act
        var (statusCode, preferences) = _usersPublicBoardPreferenceService.GetPreferences(_user.UserId).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(preferences);
    }
    
    [Fact, TestPriority(2)]
    public void AddPreference_ShouldReturnOk_WhenPreferenceAdded()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.AddPreference(_user.UserId, _campaign.CampaignGuid.Value, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(3)]
    public void GetPreferences_ShouldReturnOnePreference_WhenOnePreference()
    {
        // Arrange
        
        // Act
        var (statusCode, preferences) = _usersPublicBoardPreferenceService.GetPreferences(_user.UserId).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Single(preferences);
        Assert.Equal(_campaign.CampaignGuid, preferences.First().CampaignGuid);
        Assert.True(preferences.First().IsPreferred);
    }
    
    [Fact, TestPriority(3)]
    public void AddPreference_ShouldFailForDuplicateKey_WhenPreferenceAlreadyExists()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.AddPreference(_user.UserId, _campaign.CampaignGuid.Value, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, statusCode);
    }
    
    [Fact, TestPriority(3)]
    public void AddPreference_ShouldFailForDuplicateKey_WhenPreferenceAlreadyExistsWithDifferentValue()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.AddPreference(_user.UserId, _campaign.CampaignGuid.Value, false).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, statusCode);
    }
    
    [Fact, TestPriority(3)]
    public void AddPreference_ShouldFailForWrongUserId_WhenUserDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.AddPreference(-1, _campaign.CampaignGuid.Value, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, statusCode);
    }
    
    [Fact, TestPriority(3)]
    public void AddPreference_ShouldFailForWrongCampaignGuid_WhenCampaignDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.AddPreference(_user.UserId, Guid.Empty, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
    }
    
    [Fact, TestPriority(4)]
    public void UpdatePreference_ShouldReturnOk_WhenPreferenceUpdated()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.UpdatePreference(_user.UserId, _campaign.CampaignGuid.Value, false).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(5)]
    public void GetPreferences_ShouldReturnOnePreference_WhenOnePreferenceUpdated()
    {
        // Arrange
        
        // Act
        var (statusCode, preferences) = _usersPublicBoardPreferenceService.GetPreferences(_user.UserId).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Single(preferences);
        Assert.Equal(_campaign.CampaignGuid, preferences.First().CampaignGuid);
        Assert.False(preferences.First().IsPreferred);
    }
    
    [Fact, TestPriority(6)]
    public void GetPreferences_ShouldFailForWrongUserId_WhenUserDoesNotExist()
    {
        // Arrange
        
        // Act
        var (statusCode, preferences) = _usersPublicBoardPreferenceService.GetPreferences(-1).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, statusCode);
        Assert.Empty(preferences);
    }
    
    [Fact, TestPriority(7)]
    public void UpdatePreference_ShouldFailForWrongUserId_WhenUserDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.UpdatePreference(-1, _campaign.CampaignGuid.Value, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, statusCode);
    }
    
    [Fact, TestPriority(7)]
    public void UpdatePreference_ShouldFailForWrongCampaignGuid_WhenCampaignDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.UpdatePreference(_user.UserId, Guid.Empty, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
    }
    
    [Fact, TestPriority(7)]
    public void DeletePreference_ShouldFailForWrongUserId_WhenUserDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.RemovePreference(-1, _campaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, statusCode);
    }
    
    [Fact, TestPriority(7)]
    public void DeletePreference_ShouldFailForWrongCampaignGuid_WhenCampaignDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.RemovePreference(_user.UserId, Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
    }


    [Fact, TestPriority(100)]
    public void DeletePreference_ShouldReturnOk_WhenPreferenceDeleted()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.RemovePreference(_user.UserId, _campaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(101)]
    public void GetPreferences_ShouldReturnEmptyList_WhenNoPreferencesAfterDelete()
    {
        // Arrange
        
        // Act
        var (statusCode, preferences) = _usersPublicBoardPreferenceService.GetPreferences(_user.UserId).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(preferences);
    }
    
    [Fact, TestPriority(101)]
    public void UpdatePreference_ShouldFailForWrongCampaignGuid_WhenPreferenceDoesNotExist()
    {
        // Arrange
        
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.UpdatePreference(_user.UserId, _campaign.CampaignGuid.Value, true).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.PreferenceNotFound, statusCode);
    }
    
    [Fact, TestPriority(101)]
    public void DeletePreference_ShouldFail_WhenPreferenceDoesNotExist()
    {
        // Arrange
        
        // Act
        var statusCode = _usersPublicBoardPreferenceService.RemovePreference(_user.UserId, _campaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.PreferenceNotFound, statusCode);
    }
    
}
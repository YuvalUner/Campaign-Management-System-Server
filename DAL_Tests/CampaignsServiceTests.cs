using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class CampaignsServiceTests
{
    private readonly ICampaignsService _campaignsService;
    private readonly IConfiguration _configuration;
    private readonly IUsersService _usersService;
    
    private static readonly User TestUser = new User()
    {
        Email = "bbb",
        FirstNameEng = "Test",
        LastNameEng = "User",
        UserId = 53
    };
    
    private static readonly Campaign TestCampaign = new Campaign()
    {
        CampaignId = 0,
        CampaignName = "Test Campaign",
        CampaignDescription = "Test Campaign Description",
        IsMunicipal = true,
        CityName = "חיפה",
    };
    
    public CampaignsServiceTests()
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build(); 
        _usersService = new UsersService(new GenericDbAccess(_configuration));
        _campaignsService = new CampaignsService(new GenericDbAccess(_configuration));
    }
    
    [Fact, TestPriority(1)]
    public void CreateCampaignShouldWork()
    {
        // Arrange
        TestUser.UserId = _usersService.GetUserByEmail(TestUser.Email).Result.UserId;
        
        // Act
         TestCampaign.CampaignId = _campaignsService.AddCampaign(TestCampaign, TestUser.UserId).Result;
        
        // Assert
        Assert.True(TestCampaign.CampaignId != -1);
    }

    [Fact, TestPriority(2)]
    public void GetCampaignGuidShouldWork()
    {
        // Arrange
        
        // Act
        TestCampaign.CampaignGuid = _campaignsService.GetCampaignGuid(TestCampaign.CampaignId).Result;
        
        // Assert
        Assert.True(TestCampaign.CampaignGuid != Guid.Empty && TestCampaign.CampaignGuid != null);
    }
    
    [Fact, TestPriority(2)]
    public void GetUserCampaignsShouldWork()
    {
        // Arrange
        
        // Act
        // While this tests users service, it also tests the campaign creation
        var campaigns = _usersService.GetUserCampaigns(TestUser.UserId).Result;
        
        // Assert
        Assert.True(campaigns.Count > 0);
    }
    
    [Fact, TestPriority(2)]
    public void GetCampaignUsersShouldWork()
    {
        // Arrange
        
        // Act
        var users = _campaignsService.GetUsersInCampaign(TestCampaign.CampaignGuid).Result.ToList();
        
        // Assert
        Assert.True(users.Count > 0);
    }
    
    [Fact, TestPriority(2)]
    public void IsUserInCampaignShouldReturnTrue()
    {
        // Arrange
        
        // Act
        var isInCampaign = _campaignsService.IsUserInCampaign(TestCampaign.CampaignGuid, TestUser.UserId).Result;
        
        // Assert
        Assert.True(isInCampaign);
    }
    
    [Fact, TestPriority(2)]
    public void IsUserInCampaignShouldReturnFalse()
    {
        // Arrange
        
        // Act
        var isInCampaign = _campaignsService.IsUserInCampaign(TestCampaign.CampaignGuid, 0).Result;
        
        // Assert
        Assert.False(isInCampaign);
    }

    [Fact, TestPriority(2)]
    public void GetCampaignTypeShouldWork()
    {
        // Arrange
        var correctCampaignType = new CampaignType()
        {
            IsMunicipal = true,
            CityName = TestCampaign.CityName
        };
        
        // Act
        var campaignType = _campaignsService.GetCampaignType(TestCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Equal(correctCampaignType.CityName, campaignType.CityName);
        Assert.Equal(correctCampaignType.IsMunicipal, campaignType.IsMunicipal);
    }
    
    [Fact, TestPriority(10)]
    public void DeleteCampaignShouldWork()
    {
        // Arrange
        
        // Act
        _campaignsService.DeleteCampaign(TestCampaign.CampaignGuid);
        var campaign = _campaignsService.GetCampaignType(TestCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Null(campaign);
    }
}
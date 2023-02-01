using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class CampaignsServiceInvitesServiceTests
{
    private readonly ICampaignsService _campaignsService;
    private readonly IConfiguration _configuration;
    private readonly IUsersService _usersService;
    private readonly IInvitesService _invitesService;
    private readonly ITestOutputHelper _helper;
    
    private static readonly User TestUser = new User()
    {
        Email = "bbb",
        FirstNameEng = "Test",
        LastNameEng = "User",
        UserId = 53
    };

    private static readonly User TestUser2 = new User()
    {
        Email = "ccc",
        FirstNameEng = "Test2",
        LastNameEng = "User2",
        UserId = 72
    };
    
    private static readonly Campaign TestCampaign = new Campaign()
    {
        CampaignId = 0,
        CampaignName = "Test Campaign",
        CampaignDescription = "Test Campaign Description",
        IsMunicipal = true,
        CityName = "חיפה",
    };
    
    public CampaignsServiceInvitesServiceTests(ITestOutputHelper helper)
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build(); 
        _usersService = new UsersService(new GenericDbAccess(_configuration));
        _campaignsService = new CampaignsService(new GenericDbAccess(_configuration));
        _invitesService = new InvitesService(new GenericDbAccess(_configuration));
        _helper = helper;
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

    [Fact, TestPriority(3)]
    public void GetCampaignBasicInfoShouldWork()
    {
        // Arrange
        
        // Act
        var campaign = _campaignsService.GetCampaignBasicInfo(TestCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.NotNull(campaign);
        Assert.Equal(TestCampaign.CampaignName, campaign.CampaignName);
        Assert.Equal(TestCampaign.CampaignDescription, campaign.CampaignDescription);
        Assert.Equal(TestCampaign.IsMunicipal, campaign.IsMunicipal);
        Assert.Equal(TestCampaign.CityName, campaign.CityName);
    }
    
    [Fact, TestPriority(3)]
    public void GetCampaignBasicInfoShouldFail()
    {
        // Arrange
        
        // Act
        var campaign = _campaignsService.GetCampaignBasicInfo(Guid.Empty).Result;
        
        // Assert
        Assert.Null(campaign);
    }

    [Fact, TestPriority(4)]
    public void ModifyCampaignShouldWork()
    {
        // Arrange
        TestCampaign.CampaignDescription = "Modified Campaign Description";
        
        // Act
        _campaignsService.ModifyCampaign(TestCampaign);
        var campaign = _campaignsService.GetCampaignBasicInfo(TestCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.NotNull(campaign);
        Assert.Equal(TestCampaign.CampaignDescription, campaign.CampaignDescription);
    }

    [Fact, TestPriority(4)]
    public void GetCampaignInviteGuidShouldReturnNull()
    {
        // Arrange
        
        // Act
        var inviteGuid = _invitesService.GetInvite(TestCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Null(inviteGuid);
    }
    
    [Fact, TestPriority(5)]
    public void CreateCampaignInviteAndGetShouldWork()
    {
        // Arrange
        
        // Act
        _invitesService.CreateInvite(TestCampaign.CampaignGuid.Value).Wait();
        var inviteGuid = _invitesService.GetInvite(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.NotNull(inviteGuid);
        Assert.NotEqual(Guid.Empty, inviteGuid);
    }

    [Fact, TestPriority(5)]
    public void GetCampaignInviteGuidShouldFail()
    {
        // Arrange
        
        // Act
        var inviteGuid = _invitesService.GetInvite(Guid.Empty).Result;
        
        // Assert
        Assert.Null(inviteGuid);
    }
    
    [Fact, TestPriority(6)]
    public void RevokeCampaignInviteGuidShouldFail()
    {
        // Arrange
        
        // Act
        _invitesService.RevokeInvite(Guid.Empty);
        var inviteGuid = _invitesService.GetInvite(TestCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.NotNull(inviteGuid);
        Assert.NotEqual(Guid.Empty, inviteGuid);
    }
    
    [Fact, TestPriority(6)]
    public void GetCampaignInviteGuidShouldWork()
    {
        // Arrange
        
        // Act
        var inviteGuid = _invitesService.GetInvite(TestCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.NotNull(inviteGuid);
        Assert.NotEqual(Guid.Empty, inviteGuid);
    }

    [Fact, TestPriority(6)]
    public void GetCampaignByInviteGuidShouldWork()
    {
        // Arrange
        
        // Act
        var inviteGuid = _invitesService.GetInvite(TestCampaign.CampaignGuid.Value).Result;
        var campaign = _campaignsService.GetCampaignByInviteGuid(inviteGuid).Result;
        
        // Assert
        Assert.NotNull(campaign);
        Assert.Equal(TestCampaign.CampaignGuid, campaign.CampaignGuid);
    }
    
    [Fact, TestPriority(6)]
    public void GetCampaignByInviteGuidShouldFail()
    {
        // Arrange
        
        // Act
        var campaign = _campaignsService.GetCampaignByInviteGuid(Guid.Empty).Result;
        
        // Assert
        Assert.Null(campaign);
    }

    [Fact, TestPriority(6)]
    public void GetCampaignNameByGuidShouldWork()
    {
        // Arrange
        
        // Act
        var campaignName = _campaignsService.GetCampaignNameByGuid(TestCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.NotNull(campaignName);
        Assert.Equal(TestCampaign.CampaignName, campaignName);
    }
    
    [Fact, TestPriority(6)]
    public void GetCampaignNameByGuidShouldFail()
    {
        // Arrange
        
        // Act
        var campaignName = _campaignsService.GetCampaignNameByGuid(Guid.Empty).Result;
        
        // Assert
        Assert.Null(campaignName);
    }
    
    [Fact, TestPriority(6)]
    public void AcceptInviteShouldWork()
    {
        // Arrange

        // Act
        _invitesService.AcceptInvite(TestCampaign.CampaignGuid.Value, TestUser2.UserId).Wait();
        var users = _campaignsService.GetUsersInCampaign(TestCampaign.CampaignGuid).Result.ToList();
        
        // Assert
        Assert.True(users.Count > 1);
    }
    
    [Fact, TestPriority(7)]
    public void RevokeCampaignInviteGuidShouldWork()
    {
        // Arrange
        
        // Act
        _invitesService.RevokeInvite(TestCampaign.CampaignGuid.Value).Wait();
        var inviteGuid = _invitesService.GetInvite(TestCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Null(inviteGuid);
    }
    
    
    
    [Fact, TestPriority(10)]
    public void DeleteCampaignShouldWork()
    {
        // Arrange
        
        // Act
        _campaignsService.DeleteCampaign(TestCampaign.CampaignGuid).Wait();
        var campaign = _campaignsService.GetCampaignType(TestCampaign.CampaignGuid).Result;
        
        // Assert
        Assert.Null(campaign);
    }
}
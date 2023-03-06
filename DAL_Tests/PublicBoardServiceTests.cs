using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class PublicBoardServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IPublicBoardService _publicBoardService;
    
    private static readonly User _testUser = new User()
    {
        UserId = 1,
    };
    
    private static readonly Campaign _preferredCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("050528E2-ED6A-4192-B560-2594C9ED3370")
    };
    
    private static readonly Campaign _neutralCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("77A5A6E4-7177-4560-83C9-D559BC5210CD")
    };

    private static readonly Campaign _avoidedCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("F325C45F-16E2-4D2B-BFE9-A97A285CE47E")
    };
    
    public PublicBoardServiceTests()
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build(); 
        
        _publicBoardService = new PublicBoardService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(1)]
    public void GetEventsForUser_ShouldReturnEventsFromAllCampaigns_ForNoUserIdGiven()
    {
        // Arrange
        
        // Act
        var res = _publicBoardService.GetEventsForUser(null, null).Result;
        var resList = res.ToList();
        
        // Assert
        Assert.True(resList.Count > 0);
        Assert.Contains(resList, x => x.CampaignGuid == _preferredCampaign.CampaignGuid);
        // Once there are more events in the database, this line may need to be changed, as the neutral campaign may not be 
        // be retrieved among the top 50 anymore.
        // If that happens, the test will fail - but that is a good thing, as it means that the test is working as intended.
        // The test will need to be updated to reflect the new situation.
        Assert.Contains(resList, x => x.CampaignGuid == _neutralCampaign.CampaignGuid);
        Assert.Contains(resList, x => x.CampaignGuid == _avoidedCampaign.CampaignGuid);
        // Check that the events are ordered by date, newest first
        for (int i = 0; i < resList.Count - 2; i++)
        {
            Assert.True(resList[i].PublishingDate >= resList[i + 1].PublishingDate);
        }
    }
    
    [Fact, TestPriority(2)]
    public void GetEventsForUser_ShouldReturnEventsFromPreferredCampaigns_ForUserIdGiven()
    {
        // Arrange
        
        // Act
        var res = _publicBoardService.GetEventsForUser(_testUser.UserId, null).Result;
        var resList = res.ToList();
        
        // Assert
        Assert.True(resList.Count > 0);
        Assert.Contains(resList, x => x.CampaignGuid == _preferredCampaign.CampaignGuid);
        // Once there are more events in the database, this line may need to be changed, as the neutral campaign may not be 
        // be retrieved among the top 50 anymore.
        // If that happens, the test will fail - but that is a good thing, as it means that the test is working as intended.
        // The test will need to be updated to reflect the new situation.
        Assert.Contains(resList, x => x.CampaignGuid == _neutralCampaign.CampaignGuid);
        Assert.DoesNotContain(resList, x => x.CampaignGuid == _avoidedCampaign.CampaignGuid);
        // Flag is used to make sure that the preferred campaign events are first
        // It is set to false when the first preferred campaign event is found, and then set to true when another campaign is found.
        // If the flag is true when the next preferred campaign event is found, the test fails.
        bool flag = false;
        // Check that the events are ordered by date, newest first
        // Also check that events from the preferred campaign are first
        for (int i = 0; i < resList.Count - 2; i++)
        {
            if (resList[i].CampaignGuid == resList[i + 1].CampaignGuid)
            {
                Assert.True(resList[i].PublishingDate >= resList[i + 1].PublishingDate);
            }
            if (resList[i].CampaignGuid == _preferredCampaign.CampaignGuid)
            {
                Assert.False(flag);
                flag = false;
            }
            else
            {
                flag = true;
            }
        }
    }
    
    [Fact, TestPriority(3)]
    public void GetAnnouncementsForUser_ShouldReturnAnnouncementsFromAllCampaigns_ForNoUserIdGiven()
    {
        // Arrange
        
        // Act
        var res = _publicBoardService.GetAnnouncementsForUser(null, null).Result;
        var resList = res.ToList();
        
        // Assert
        Assert.True(resList.Count > 0);
        Assert.Contains(resList, x => x.CampaignGuid == _preferredCampaign.CampaignGuid);
        // Once there are more events in the database, this line may need to be changed, as the neutral campaign may not be 
        // be retrieved among the top 50 anymore.
        // If that happens, the test will fail - but that is a good thing, as it means that the test is working as intended.
        // The test will need to be updated to reflect the new situation.
        Assert.Contains(resList, x => x.CampaignGuid == _neutralCampaign.CampaignGuid);
        Assert.Contains(resList, x => x.CampaignGuid == _avoidedCampaign.CampaignGuid);
        // Check that the announcements are ordered by date, newest first
        for (int i = 0; i < resList.Count - 2; i++)
        {
            Assert.True(resList[i].PublishingDate >= resList[i + 1].PublishingDate);
        }
    }
    
    [Fact, TestPriority(4)]
    public void GetAnnouncementsForUser_ShouldReturnAnnouncementsFromPreferredCampaigns_ForUserIdGiven()
    {
        // Arrange
        
        // Act
        var res = _publicBoardService.GetAnnouncementsForUser(_testUser.UserId, null).Result;
        var resList = res.ToList();
        
        // Assert
        Assert.True(resList.Count > 0);
        Assert.Contains(resList, x => x.CampaignGuid == _preferredCampaign.CampaignGuid);
        // Once there are more events in the database, this line may need to be changed, as the neutral campaign may not be 
        // be retrieved among the top 50 anymore.
        // If that happens, the test will fail - but that is a good thing, as it means that the test is working as intended.
        // The test will need to be updated to reflect the new situation.
        Assert.Contains(resList, x => x.CampaignGuid == _neutralCampaign.CampaignGuid);
        Assert.DoesNotContain(resList, x => x.CampaignGuid == _avoidedCampaign.CampaignGuid);
        // Flag is used to make sure that the preferred campaign announcements are first
        // It is set to false when the first preferred campaign announcement is found, and then set to true when another campaign is found.
        // If the flag is true when the next preferred campaign announcement is found, the test fails.
        bool flag = false;
        // Check that the announcements are ordered by date, newest first
        // Also check that announcements from the preferred campaign are first
        for (int i = 0; i < resList.Count - 2; i++)
        {
            if (resList[i].CampaignGuid == resList[i + 1].CampaignGuid)
            {
                Assert.True(resList[i].PublishingDate >= resList[i + 1].PublishingDate);
            }
            if (resList[i].CampaignGuid == _preferredCampaign.CampaignGuid)
            {
                Assert.False(flag);
                flag = false;
            }
            else
            {
                flag = true;
            }
        }
    }
}
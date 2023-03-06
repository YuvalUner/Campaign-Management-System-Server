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
        FirstNameHeb = "יובל",
        LastNameHeb = "אונר",
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
        CampaignGuid = Guid.Parse("F325C45F-16E2-4D2B-BFE9-A97A285CE47E"),
        CityName = "ארצי"
    };

    private static readonly CustomEvent _testEvent = new CustomEvent()
    {
        EventName = "Test Event",
        EventStartTime = DateTime.Parse("2023-03-06 17:25:12.473"),
        EventEndTime = DateTime.Parse("2024-03-06 17:25:12.473"),
        EventLocation = "Test"
    };

    private static DateTime _testEventPublishingDate = DateTime.Parse("2023-03-06 13:43:03.580");

    private static Announcement _testAnnouncement = new Announcement()
    {
        AnnouncementTitle = "Test"
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

    [Fact, TestPriority(5)]
    public void SearchPublishedEvents_ShouldReturnEmptyList_ForWrongCampaignGuid()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            CampaignGuid = Guid.Empty
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(6)]
    public void SearchPublishedEvents_ShouldReturnEmptyList_ForPublisherFirstName()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            PublisherFirstName = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(7)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectCampaignGuid()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            CampaignGuid = _preferredCampaign.CampaignGuid
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_preferredCampaign.CampaignGuid, x.CampaignGuid));
    }
    
    [Fact, TestPriority(8)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectPublisherFirstName()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            PublisherFirstName = _testUser.FirstNameHeb
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testUser.FirstNameHeb, x.FirstNameHeb));
    }
    
    [Fact, TestPriority(9)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectPublisherLastName()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            PublisherLastName = _testUser.LastNameHeb
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testUser.LastNameHeb, x.LastNameHeb));
    }

    [Fact, TestPriority(10)]
    public void SearchPublishdEvents_ShouldReturnEmpty_ForWrongPublisherLastName()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            PublisherLastName = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(11)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectEventName()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventName = _testEvent.EventName
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testEvent.EventName, x.EventName));
    }
    
    [Fact, TestPriority(12)]
    public void SearchPublishedEvents_ShouldReturnEmpty_ForWrongEventName()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventName = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }

    [Fact, TestPriority(13)]
    public void SearchPublishedEvents_ShouldReturnsEvents_ForCorrectEventLocation()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventLocation = _testEvent.EventLocation
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testEvent.EventLocation, x.EventLocation));
    }
    
    [Fact, TestPriority(14)]
    public void SearchPublishedEvents_ShouldReturnEmpty_ForWrongEventLocation()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventLocation = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }

    [Fact, TestPriority(15)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectEventStartTime()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventStartTime = _testEvent.EventStartTime
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testEvent.EventStartTime, x.EventStartTime));
    }
    
    [Fact, TestPriority(16)]
    public void SearchPublishedEvents_ShouldReturnEmpty_ForWrongEventStartTime()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventStartTime = DateTime.Parse("01/01/2100")
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(17)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectEventEndTime()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventEndTime = _testEvent.EventEndTime
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.True(_testEvent.EventEndTime <= x.EventEndTime));
    }
    
    [Fact, TestPriority(18)]
    public void SearchPublishedEvents_ShouldReturnEmpty_ForWrongEventEndTime()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            EventEndTime = DateTime.Parse("01/01/1900")
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }

    [Fact, TestPriority(19)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectPublishingDate()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            PublishingDate = _testEventPublishingDate
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testEventPublishingDate.Date, x.PublishingDate.Value.Date));
    }
    
    [Fact, TestPriority(20)]
    public void SearchPublishedEvents_ShouldReturnEmpty_ForWrongPublishingDate()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            PublishingDate = DateTime.Parse("01/01/1900")
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    

    [Fact, TestPriority(21)]
    public void SearchPublishedAnnouncements_ShouldReturnAnnouncements_ForCorrectCampaignGuid()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            CampaignGuid = _preferredCampaign.CampaignGuid
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_preferredCampaign.CampaignGuid, x.CampaignGuid));
    }
    
    [Fact, TestPriority(22)]
    public void SearchPublishedAnnouncements_ShouldReturnAnnouncements_ForCorrectPublisherFirstName()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            PublisherFirstName = _testUser.FirstNameHeb
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testUser.FirstNameHeb, x.FirstNameHeb));
    }
    
    [Fact, TestPriority(23)]
    public void SearchPublishedAnnouncements_ShouldReturnEmpty_ForWrongPublisherFirstName()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            PublisherFirstName = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(24)]
    public void SearchPublishedAnnouncements_ShouldReturnAnnouncements_ForCorrectPublisherLastName()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            PublisherLastName = _testUser.LastNameHeb
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testUser.LastNameHeb, x.LastNameHeb));
    }
    
    [Fact, TestPriority(25)]
    public void SearchPublishedAnnouncements_ShouldReturnEmpty_ForWrongPublisherLastName()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            PublisherLastName = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }

    [Fact, TestPriority(26)]
    public void SearchPublishedAnnouncements_ShouldReturnEmpty_ForWrongCampaignGuid()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            CampaignGuid = Guid.Empty
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(27)]
    public void SearchPublishedAnnouncements_ShouldReturnAnnouncements_ForCorrectAnnouncementTitle()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            AnnouncementTitle = _testAnnouncement.AnnouncementTitle
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testAnnouncement.AnnouncementTitle, x.AnnouncementTitle));
    }
    
    [Fact, TestPriority(28)]
    public void SearchPublishedAnnouncements_ShouldReturnEmpty_ForWrongAnnouncementTitle()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            AnnouncementTitle = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }

    [Fact, TestPriority(29)]
    public void SearchPublishedAnnouncements_ShouldReturnAnnouncements_ForCorrectPublishingDate()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            PublishingDate = _testEventPublishingDate
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
        Assert.All(res, x => Assert.Equal(_testEventPublishingDate.Date, x.PublishingDate.Value.Date));
    }
    
    [Fact, TestPriority(30)]
    public void SearchPublishedAnnouncements_ShouldReturnEmpty_ForWrongPublishingDate()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            PublishingDate = DateTime.Parse("01/01/1900")
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }

    [Fact, TestPriority(31)]
    public void SearchPublishedEvents_ShouldReturnEvents_ForCorrectCampaignCity()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            CampaignCity = _preferredCampaign.CityName
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
    }
    
    [Fact, TestPriority(32)]
    public void SearchPublishedEvents_ShouldReturnEmpty_ForWrongCampaignCity()
    {
        // Arrange
        var EventsSearchParams = new EventsSearchParams()
        {
            CampaignCity = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchEvents(EventsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
    
    [Fact, TestPriority(33)]
    public void SearchPublishedAnnouncements_ShouldReturnAnnouncements_ForCorrectCampaignCity()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            CampaignCity = _preferredCampaign.CityName
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.NotEmpty(res);
    }
    
    [Fact, TestPriority(34)]
    public void SearchPublishedAnnouncements_ShouldReturnEmpty_ForWrongCampaignCity()
    {
        // Arrange
        var AnnouncementsSearchParams = new AnnouncementsSearchParams()
        {
            CampaignCity = "wrong search string"
        };
        
        // Act
        var res = _publicBoardService.SearchAnnouncements(AnnouncementsSearchParams).Result;
        
        // Assert
        Assert.Empty(res);
    }
}
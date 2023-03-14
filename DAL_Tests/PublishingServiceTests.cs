using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

/// <summary>
/// A collection of tests for the <see cref="IPublishingService"/> interface and its implementation, <see cref="PublishingService"/>.<br/>
/// The tests are executed in a sequential order, as defined by the <see cref="PriorityOrderer"/>,
/// using the <see cref="TestPriorityAttribute"/> attribute.
/// </summary>
[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class PublishingServiceTests
{
    private readonly IPublishingService _publishingService;
    private readonly IConfiguration _configuration;

    private static User _testUser = new User()
    {
        UserId = 53,
        Email = "bbb",
    };

    private static Campaign _testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6")
    };

    static CustomEvent _testEvent = new CustomEvent()
    {
        EventGuid = Guid.Parse("21B6E907-CDE2-4E33-B425-733C027D201B")
    };

    static CustomEvent _nonCampaignEvent = new CustomEvent()
    {
        EventGuid = Guid.Parse("736019CC-5125-4DA4-8E6B-A07E79464993")
    };

    static Announcement _testAnnouncement = new Announcement()
    {
        AnnouncementTitle = "Test Announcement",
        AnnouncementContent = "Test Announcement Content",
        PublisherId = _testUser.UserId
    };

    public PublishingServiceTests()
    {
        _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        _publishingService = new PublishingService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(1)]
    public void GetCampaignPublishedEvents_ShouldReturnEmptyList_ForNoEventsYet()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignPublishedEvents(_testCampaign.CampaignGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(res);
    }

    [Fact, TestPriority(1)]
    public void GetCampaignPublishedAnnouncements_ShouldReturnEmptyList_ForNoEventsYet()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignAnnouncements(_testCampaign.CampaignGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(res);
    }

    [Fact, TestPriority(2)]
    public void PublishEventShouldWork()
    {
        // Arrange

        // Act
        var result = _publishingService.PublishEvent(_testEvent.EventGuid, _testUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(3)]
    public void PublishEventShouldFail_ForDuplicateKey()
    {
        // Arrange

        // Act
        var result = _publishingService.PublishEvent(_testEvent.EventGuid, _testUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, result);
    }

    [Fact, TestPriority(3)]
    public void PublishEventShouldFail_ForInvalidEventGuid()
    {
        // Arrange

        // Act
        var result = _publishingService.PublishEvent(Guid.Empty, _testUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventNotFound, result);
    }

    [Fact, TestPriority(3)]
    public void PublishEventShouldFail_ForInvalidPublisherId()
    {
        // Arrange

        // Act
        var result = _publishingService.PublishEvent(_testEvent.EventGuid, 0).Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }

    [Fact, TestPriority(3)]
    public void UnpublishEventShouldFail_ForInvalidEventGuid()
    {
        // Arrange

        // Act
        var result = _publishingService.UnpublishEvent(Guid.Empty).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventNotFound, result);
    }

    [Fact, TestPriority(3)]
    public void PublishEventShouldFail_ForWrongEventType()
    {
        // Arrange

        // Act
        var result = _publishingService.PublishEvent(_nonCampaignEvent.EventGuid, _testUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.IncorrectEventType, result);
    }

    [Fact, TestPriority(3)]
    public void GetCampaignPublishedEventsShouldWork()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignPublishedEvents(_testCampaign.CampaignGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Single(res);
        Assert.Equal(_testEvent.EventGuid, res.First().EventGuid);
        Assert.Equal(_testUser.Email, res.First().Email);
        Assert.Equal(_testCampaign.CampaignGuid, res.First().CampaignGuid);
    }

    [Fact, TestPriority(3)]
    public void GetCampaignPublishedEventsShouldFail_ForInvalidCampaignGuid()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignPublishedEvents(Guid.Empty).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
        Assert.Empty(res);
    }

    [Fact, TestPriority(4)]
    public void PublishAnnouncementShouldWork()
    {
        // Arrange

        // Act
        var (statusCode, newGuid) = _publishingService
            .PublishAnnouncement(_testAnnouncement, _testCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.NotEqual(Guid.Empty, newGuid);
        _testAnnouncement.AnnouncementGuid = newGuid;
    }

    [Fact, TestPriority(4)]
    public void PublishAnnouncementShouldFail_ForInvalidCampaignGuid()
    {
        // Arrange

        // Act
        var (statusCode, newGuid) = _publishingService.PublishAnnouncement(_testAnnouncement, Guid.Empty).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
        Assert.Equal(Guid.Empty, newGuid);
    }

    [Fact, TestPriority(4)]
    public void PublishAnnouncementShouldFail_ForInvalidPublisherId()
    {
        // Arrange
        var failedPublishingAnnouncement = new Announcement()
        {
            AnnouncementTitle = "Test Announcement",
            AnnouncementContent = "Test Announcement Content",
            PublisherId = 0
        };

        // Act
        var (statusCode, newGuid) = _publishingService
            .PublishAnnouncement(failedPublishingAnnouncement, _testCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, statusCode);
        Assert.Equal(Guid.Empty, newGuid);
    }

    [Fact, TestPriority(5)]
    public void GetCampaignPublishedAnnouncementsShouldWork()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignAnnouncements(_testCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Single(res);
        Assert.Equal(_testAnnouncement.AnnouncementGuid, res.First().AnnouncementGuid);
        Assert.Equal(_testAnnouncement.AnnouncementTitle, res.First().AnnouncementTitle);
        Assert.Equal(_testAnnouncement.AnnouncementContent, res.First().AnnouncementContent);
        Assert.Equal(_testUser.Email, res.First().Email);
        Assert.Equal(_testCampaign.CampaignGuid, res.First().CampaignGuid);
    }

    [Fact, TestPriority(5)]
    public void GetCampaignPublishedAnnouncementsShouldFail_ForInvalidCampaignGuid()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignAnnouncements(Guid.Empty).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
        Assert.Empty(res);
    }

    [Fact, TestPriority(100)]
    public void UnpublishEventShouldWork()
    {
        // Arrange

        // Act
        var result = _publishingService.UnpublishEvent(_testEvent.EventGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(101)]
    public void UnpublishEventShouldFail_ForRemovingAlreadyUnpublishedEvent()
    {
        // Arrange

        // Act
        var result = _publishingService.UnpublishEvent(_testEvent.EventGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventNotFound, result);
    }

    [Fact, TestPriority(101)]
    public void GetEventsForCampaign_ShouldReturnEmptyList_AfterUnpublishing()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignPublishedEvents(_testCampaign.CampaignGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(res);
    }

    [Fact, TestPriority(100)]
    public void UnpublishAnnouncementShouldWork()
    {
        // Arrange

        // Act
        var result = _publishingService.UnpublishAnnouncement(_testAnnouncement.AnnouncementGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(101)]
    public void UnpublishAnnouncementShouldFail_ForRemovingAlreadyUnpublishedAnnouncement()
    {
        // Arrange

        // Act
        var result = _publishingService.UnpublishAnnouncement(_testAnnouncement.AnnouncementGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.AnnouncementNotFound, result);
    }

    [Fact, TestPriority(101)]
    public void GetCampaignPublishedAnnouncements_ShouldReturnEmptyList_AfterUnpublishing()
    {
        // Arrange

        // Act
        var (statusCode, res) = _publishingService.GetCampaignAnnouncements(_testCampaign.CampaignGuid).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(res);
    }
}
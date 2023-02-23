using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class EventServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IEventsService _eventsService;

    private static User testUser = new User()
    {
        UserId = 53,
        Email = "bbb",
    };

    private static User secondTestUser = new User()
    {
        UserId = 72,
        Email = "ccc",
    };

    private static Campaign testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6"),
        CampaignId = 52
    };

    private static CustomEvent testCustomEvent = new CustomEvent()
    {
        EventName = "Test",
        EventDescription = "Test",
        EventStartTime = DateTime.Parse("2021-01-01 00:00:00"),
        EventEndTime = DateTime.Parse("2021-01-02 00:00:00"),
        EventLocation = "Test",
        EventCreatorId = testUser.UserId,
        MaxAttendees = 1
    };

    public EventServiceTests()
    {
        _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        _eventsService = new EventsService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(0)]
    public void GetEventsForUserShouldReturnEmpty()
    {
        // Arrange

        // Act
        var events = _eventsService.GetUserEvents(testUser.UserId).Result;

        // Assert
        Assert.NotNull(events);
        Assert.Empty(events);
    }

    [Fact, TestPriority(0)]
    public void GetEventsForCampaignShouldReturnEmpty()
    {
        // Arrange

        // Act
        var events = _eventsService.GetCampaignEvents(testCampaign.CampaignGuid).Result;

        // Assert
        Assert.NotNull(events.Item2);
        Assert.Empty(events.Item2);
        Assert.Equal(CustomStatusCode.Ok, events.Item1);
    }

    [Fact, TestPriority(1)]
    public void AddEventShouldWork()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEvent(testCustomEvent).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, res.Item1);
        Assert.NotNull(res.Item2);
        Assert.NotNull(res.Item3);
        testCustomEvent.EventGuid = res.Item3;
        testCustomEvent.EventId = res.Item2;
    }

    [Fact, TestPriority(1)]
    public void AddEventShouldFailForWrongCampaignGuid()
    {
        // Arrange
        var customEvent = new CustomEvent()
        {
            EventName = "Test",
            EventDescription = "Test",
            EventStartTime = DateTime.Now,
            EventEndTime = DateTime.Now + TimeSpan.FromDays(1),
            EventLocation = "Test",
            EventCreatorId = testUser.UserId,
            CampaignGuid = Guid.Empty
        };

        // Act
        var res = _eventsService.AddEvent(customEvent).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, res.Item1);
        Assert.Equal(0, res.Item2);
        Assert.Equal(Guid.Empty, res.Item3);
    }

    [Fact, TestPriority(2)]
    public void UpdateEventShouldWork()
    {
        // Arrange
        var customEvent = new CustomEvent()
        {
            EventName = "Test2",
            EventDescription = "Test2",
            EventStartTime = DateTime.Parse("2022-01-01 00:00:00"),
            EventEndTime = DateTime.Parse("2022-01-02 00:00:00"),
            EventLocation = "Test2",
            EventCreatorId = testUser.UserId,
            EventGuid = testCustomEvent.EventGuid,
            EventId = testCustomEvent.EventId,
            MaxAttendees = testCustomEvent.MaxAttendees,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var res = _eventsService.UpdateEvent(customEvent).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, res);
        testCustomEvent = customEvent;
    }

    [Fact, TestPriority(2)]
    public void UpdateEventShouldFailForWrongCampaignGuid()
    {
        // Arrange
        var customEvent = new CustomEvent()
        {
            EventName = "Test2",
            EventDescription = "Test2",
            EventStartTime = DateTime.Now,
            EventEndTime = DateTime.Now + TimeSpan.FromDays(1),
            EventLocation = "Test2",
            EventCreatorId = testUser.UserId,
            EventGuid = testCustomEvent.EventGuid,
            EventId = testCustomEvent.EventId,
            CampaignGuid = Guid.Empty
        };

        // Act
        var res = _eventsService.UpdateEvent(customEvent).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, res);
    }

    [Fact, TestPriority(2)]
    public void UpdateEventShouldFailForWrongEventGuid()
    {
        // Arrange
        var customEvent = new CustomEvent()
        {
            EventName = "Test2",
            EventDescription = "Test2",
            EventStartTime = DateTime.Now,
            EventEndTime = DateTime.Now + TimeSpan.FromDays(1),
            EventLocation = "Test2",
            EventCreatorId = testUser.UserId,
            EventGuid = Guid.Empty,
            EventId = testCustomEvent.EventId
        };

        // Act
        var res = _eventsService.UpdateEvent(customEvent).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventNotFound, res);
    }

    [Fact, TestPriority(3)]
    public void GetEventShouldWork()
    {
        // Arrange

        // Act
        var res = _eventsService.GetEvent(testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.NotNull(res);
        Assert.Equal(testCustomEvent.EventName, res.EventName);
        Assert.Equal(testCustomEvent.EventDescription, res.EventDescription);
        Assert.Equal(testCustomEvent.EventStartTime, res.EventStartTime);
        Assert.Equal(testCustomEvent.EventEndTime, res.EventEndTime);
        Assert.Equal(testCustomEvent.EventLocation, res.EventLocation);
        Assert.Equal(testCustomEvent.MaxAttendees, res.MaxAttendees);
        Assert.Equal(testUser.Email, res.Email);
    }

    [Fact, TestPriority(3)]
    public void GetEventShouldFailForWrongEventGuid()
    {
        // Arrange

        // Act
        var res = _eventsService.GetEvent(Guid.Empty).Result;

        // Assert
        Assert.Null(res);
    }

    [Fact, TestPriority(3)]
    public void GetEventsForCampaignShouldReturnOne()
    {
        // Arrange

        // Act
        var events = _eventsService.GetCampaignEvents(testCampaign.CampaignGuid).Result;

        // Assert
        Assert.NotNull(events.Item2);
        Assert.Single(events.Item2);
        Assert.Equal(CustomStatusCode.Ok, events.Item1);
        Assert.Contains(events.Item2, x => x.EventGuid == testCustomEvent.EventGuid);
    }

    [Fact, TestPriority(3)]
    public void AddWatcherForEventShouldWork()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventWatcher(testUser.UserId, testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, res);
    }

    [Fact, TestPriority(3)]
    public void AddWatcherForEventShouldFailForWrongEventGuid()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventWatcher(testUser.UserId, Guid.Empty).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventNotFound, res);
    }

    [Fact, TestPriority(3)]
    public void AddWatcherForEventShouldFailForWrongUserId()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<System.AggregateException>(() => _eventsService.AddEventWatcher(0,
            testCustomEvent.EventGuid.Value).Result);
    }

    [Fact, TestPriority(4)]
    public void AddWatcherForEventShouldFailForDuplicateKey()
    {
        // Arrange
        
        // Act
        var res = _eventsService.AddEventWatcher(testUser.UserId, testCustomEvent.EventGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, res);
    }

    [Fact, TestPriority(4)]
    public void GetEventsForUserShouldReturnOneNotParticipating()
    {
        // Arrange

        // Act
        var events = _eventsService.GetUserEvents(testUser.UserId).Result.ToList();

        // Assert
        Assert.NotNull(events);
        Assert.Single(events);
        Assert.Equal(testCustomEvent.EventName, events[0].EventName);
        Assert.Equal(testCustomEvent.EventDescription, events[0].EventDescription);
        Assert.Equal(testCustomEvent.EventStartTime, events[0].EventStartTime);
        Assert.Equal(testCustomEvent.EventEndTime, events[0].EventEndTime);
        Assert.Equal(testCustomEvent.EventLocation, events[0].EventLocation);
        Assert.Equal(testCustomEvent.EventGuid, events[0].EventGuid);
        Assert.False(events[0].Participating);
    }

    [Fact, TestPriority(5)]
    public void GetEventParticipantsShouldReturnEmpty()
    {
        // Arrange

        // Act
        var res = _eventsService.GetEventParticipants(testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.NotNull(res.Item2);
        Assert.Empty(res.Item2);
        Assert.Equal(CustomStatusCode.Ok, res.Item1);
    }

    [Fact, TestPriority(5)]
    public void GetEventParticipantsShouldFailForWrongEventGuid()
    {
        // Arrange

        // Act
        var res = _eventsService.GetEventParticipants(Guid.Empty).Result;

        // Assert
        Assert.NotNull(res.Item2);
        Assert.Empty(res.Item2);
        Assert.Equal(CustomStatusCode.EventNotFound, res.Item1);
    }

    [Fact, TestPriority(6)]
    public void AddParticipantForEventShouldWork()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventParticipant(testCustomEvent.EventGuid.Value,
            userId: testUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, res);
    }

    [Fact, TestPriority(6)]
    public void AddParticipantForEventShouldFailForWrongEventGuid()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventParticipant(Guid.Empty,
            userId: testUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventNotFound, res);
    }

    [Fact, TestPriority(6)]
    public void AddParticipantForEventShouldFailForWrongUserId()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<System.AggregateException>(() => _eventsService.AddEventParticipant(
            testCustomEvent.EventGuid.Value,
            userId: 0).Result);
    }

    [Fact, TestPriority(6)]
    public void AddParticipantForEventShouldFailForEmptyUserIdAndEmail()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventParticipant(testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.ParameterMustNotBeNullOrEmpty, res);
    }

    [Fact, TestPriority(6)]
    public void AddParticipantForEventShouldFailForWrongEmail()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventParticipant(testCustomEvent.EventGuid.Value,
            userEmail: "wrongEmail").Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, res);
    }

    [Fact, TestPriority(6)]
    public void AddParticipantForEventShouldFailProvidingBothUserIdAndEmail()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventParticipant(testCustomEvent.EventGuid.Value,
            userEmail: "wrongEmail", userId: 0).Result;

        // Assert
        Assert.Equal(CustomStatusCode.TooManyValuesProvided, res);
    }

    [Fact, TestPriority(7)]
    public void GetEventParticipantsShouldReturnOne()
    {
        // Arrange

        // Act
        var res = _eventsService.GetEventParticipants(testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.NotNull(res.Item2);
        Assert.Single(res.Item2);
        Assert.Equal(CustomStatusCode.Ok, res.Item1);
        Assert.Contains(res.Item2, x => x.Email == testUser.Email);
    }

    [Fact, TestPriority(8)]
    public void AddParticipantShouldFailForTooManyParticipants()
    {
        // Arrange

        // Act
        var res = _eventsService.AddEventParticipant(testCustomEvent.EventGuid.Value,
            userEmail: secondTestUser.Email).Result;

        // Assert
        Assert.Equal(CustomStatusCode.EventAlreadyFull, res);
    }

    [Fact, TestPriority(9)]
    public void GetEventsForUserShouldReturnOneParticipating()
    {
        // Arrange

        // Act
        var events = _eventsService.GetUserEvents(testUser.UserId).Result.ToList();

        // Assert
        Assert.NotNull(events);
        Assert.Single(events);
        Assert.Equal(testCustomEvent.EventName, events[0].EventName);
        Assert.Equal(testCustomEvent.EventDescription, events[0].EventDescription);
        Assert.Equal(testCustomEvent.EventStartTime, events[0].EventStartTime);
        Assert.Equal(testCustomEvent.EventEndTime, events[0].EventEndTime);
        Assert.Equal(testCustomEvent.EventLocation, events[0].EventLocation);
        Assert.Equal(testCustomEvent.EventGuid, events[0].EventGuid);
        Assert.True(events[0].Participating);
    }


    [Fact, TestPriority(100)]
    public void DeleteEventShouldWork()
    {
        // Arrange

        // Act
        var res = _eventsService.DeleteEvent(testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, res);
    }

    [Fact, TestPriority(101)]
    public void GetEventParticipantsAfterDeleteShouldReturnEmpty()
    {
        // Arrange

        // Act
        var res = _eventsService.GetEventParticipants(testCustomEvent.EventGuid.Value).Result;

        // Assert
        Assert.NotNull(res.Item2);
        Assert.Empty(res.Item2);
        Assert.Equal(CustomStatusCode.EventNotFound, res.Item1);
    }
}
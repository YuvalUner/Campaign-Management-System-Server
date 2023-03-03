using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

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
    
    public PublishingServiceTests()
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build(); 
        
        _publishingService = new PublishingService(new GenericDbAccess(_configuration));
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
}
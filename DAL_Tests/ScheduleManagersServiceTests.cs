using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class ScheduleManagersServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IScheduleManagersService _scheduleManagersService;

    private static User _managedUser = new()
    {
        UserId = 2,
        Email = "aaa"
    };
    
    private static User _managerUser = new()
    {
        UserId = 53,
        Email = "bbb"
    };
    
    public ScheduleManagersServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        _scheduleManagersService = new ScheduleManagersService(new GenericDbAccess(_configuration));
    }
    
    [Fact, TestPriority(1)]
    public void GetScheduleManagersById_ShouldReturnEmptyList_WhenNoScheduleManagers()
    {
        // Arrange
        
        // Act
        var (statusCode, scheduleManagers) = _scheduleManagersService.GetScheduleManagers(userId: _managedUser.UserId).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(scheduleManagers);
    }
    
    [Fact, TestPriority(1)]
    public void GetScheduleManagersByEmail_ShouldReturnEmptyList_WhenNoScheduleManagers()
    {
        // Arrange
        
        // Act
        var (statusCode, scheduleManagers) = _scheduleManagersService.GetScheduleManagers(userEmail: _managedUser.Email).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.Empty(scheduleManagers);
    }

    [Fact, TestPriority(1)]
    public void GetScheduleManagersByEmailAndId_ShouldFail_ReturnTooManyValuesProvided()
    {
        // Arrange
        
        // Act
        var (statusCode, scheduleManagers) = _scheduleManagersService.GetScheduleManagers(userEmail: _managedUser.Email, userId: _managedUser.UserId).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.TooManyValuesProvided, statusCode);
        Assert.Empty(scheduleManagers);
    }
    
    [Fact, TestPriority(1)]
    public void GetScheduleManagersByNoValues_ShouldFail_ReturnParameterMustNotBeNullOrEmpty()
    {
        // Arrange
        
        // Act
        var (statusCode, scheduleManagers) = _scheduleManagersService.GetScheduleManagers().Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.ParameterMustNotBeNullOrEmpty, statusCode);
        Assert.Empty(scheduleManagers);
    }
    
    [Fact, TestPriority(1)]
    public void GetManagedUsers_ShouldReturnEmptyList_WhenNoManagedUsers()
    {
        // Arrange
        
        // Act
        var managedUsers = _scheduleManagersService.GetManagedUsers(_managerUser.UserId).Result;
        
        // Assert
        Assert.Empty(managedUsers);
    }
    
    [Fact, TestPriority(2)]
    public void AddScheduleManager_ShouldReturnOk_WhenScheduleManagerAdded()
    {
        // Arrange
        
        // Act
        var statusCode = _scheduleManagersService.AddScheduleManager(_managerUser.UserId, _managedUser.Email).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(100)]
    public void RemoveScheduleManagerShouldWork()
    {
        // Arrange
        
        // Act
        var statusCode = _scheduleManagersService.RemoveScheduleManager(_managerUser.UserId, _managedUser.Email).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
}
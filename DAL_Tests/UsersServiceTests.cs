using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DAL_Tests;



[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class UsersServiceTests
{
    private readonly IUsersService _usersService;
    private readonly IConfiguration _configuration;

    private static readonly User TestUser = new User()
    {
        Email = "AAAA",
        FirstNameEng = "Test",
        LastNameEng = "User",
        ProfilePicUrl = "https://www.google.com",
        UserId = 0
    };
    
    public UsersServiceTests()
    {
         _configuration = new ConfigurationBuilder().
             SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build(); 
        _usersService = new UsersService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(1)]
    public void AddUserShouldWork()
    {
        // Arrange
        
        // Act
        int newUserId = _usersService.CreateUser(TestUser).Result;
        
        // Assert
        Assert.True(newUserId > 0);
        TestUser.UserId = newUserId;
    }
    
    [Fact, TestPriority(2)]
    public void AddUserShouldThrowException()
    {
        // Arrange
        
        // Act

        // Assert
        Assert.Throws<AggregateException>(() =>  _usersService.CreateUser(TestUser).Result );
    }
    
    
    [Fact, TestPriority(2)]
    public void GetUserShouldWork()
    {
        // Arrange

        // Act
        User user = _usersService.GetUserByEmail(TestUser.Email).Result;

        // Assert
        Assert.NotNull(user);
        Assert.True(TestUser.Email == user.Email && TestUser.FirstNameEng == user.FirstNameEng && TestUser.LastNameEng == user.LastNameEng && TestUser.ProfilePicUrl == user.ProfilePicUrl);
    }
    
    [Fact, TestPriority(2)]
    public void GetUserShouldFail()
    {
        // Arrange

        // Act
        User user = _usersService.GetUserByEmail("AAAAA").Result;

        // Assert
        Assert.Null(user);
    }

    [Fact, TestPriority(2)]
    public void GetUserPublicInfoShouldWork()
    {
        var userPublicInfo = _usersService.GetUserPublicInfo(TestUser.UserId).Result;
        
        Assert.NotNull(userPublicInfo);
        Assert.True(TestUser.FirstNameEng == userPublicInfo.FirstNameEng && TestUser.LastNameEng == userPublicInfo.LastNameEng && TestUser.ProfilePicUrl == userPublicInfo.ProfilePicUrl);
    }
    
    [Fact, TestPriority(2)]
    public void GetUserPublicInfoShouldFail()
    {
        var userPublicInfo = _usersService.GetUserPublicInfo(-1).Result;
        
        Assert.Null(userPublicInfo);
    }

    [Fact, TestPriority(2)]
    public void IsUserAuthenticatedShouldReturnFalse()
    {
        // Arrange
        
        // Act
        var isAuthenticated = _usersService.IsUserAuthenticated(TestUser.UserId).Result;
        
        // Assert
        Assert.False(isAuthenticated);
    }
    
    [Fact, TestPriority(3)]
    public void VerifyUserPrivateInfoShouldWork()
    {
        // Arrange
        var info = new UserPrivateInfo()
        {
            FirstNameEng = TestUser.FirstNameEng,
            LastNameEng = TestUser.LastNameEng,
            IdNumber = 1,
            CityName = "גבעתיים"
        };
        
        // Act
        var userPrivateInfoResult = _usersService.AddUserPrivateInfo(info, TestUser.UserId).Result;
        
        // Assert
        Assert.True(userPrivateInfoResult == CustomStatusCode.Ok);
    }

    [Fact, TestPriority(4)]
    public void IsUserAuthenticatedShouldReturnTrue()
    {
        // Arrange
        
        // Act
        var isAuthenticated = _usersService.IsUserAuthenticated(TestUser.UserId).Result;
        
        // Assert
        Assert.True(isAuthenticated);
    }
    
    [Fact, TestPriority(4)]
    public void VerifyUserPrivateInfoShouldFailForDuplicate()
    {
        // Arrange
        var info = new UserPrivateInfo()
        {
            FirstNameEng = TestUser.FirstNameEng,
            LastNameEng = TestUser.LastNameEng,
            IdNumber = 1,
            CityName = "גבעתיים"
        };
        
        // Act
        var userPrivateInfoResult = _usersService.AddUserPrivateInfo(info, TestUser.UserId).Result;
        
        // Assert
        Assert.True(userPrivateInfoResult == CustomStatusCode.IdAlreadyExistsWhenVerifyingInfo);
    }
    

    [Fact, TestPriority(10)] 
    public void DeleteUserShouldWork()
    {
        _usersService.DeleteUser(TestUser.UserId).Wait();
        User? user = _usersService.GetUserByEmail(TestUser.Email).Result;
        
        Assert.Null(user);
    }
}
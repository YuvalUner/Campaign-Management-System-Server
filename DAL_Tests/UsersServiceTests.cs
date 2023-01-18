using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
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

    private static readonly User testUser = new User()
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
    public void Test1_AddUserShouldWork()
    {
        // Arrange
        
        // Act
        int newUserId = _usersService.CreateUser(testUser).Result;
        
        // Assert
        Assert.True(newUserId > 0);
        testUser.UserId = newUserId;
    }
    
    [Fact, TestPriority(2)]
    public void Test2_GetUserShouldWork()
    {
        // Arrange

        // Act
        User user = _usersService.GetUserByEmail(testUser.Email).Result;

        // Assert
        Assert.NotNull(user);
        Assert.True(testUser.Email == user.Email && testUser.FirstNameEng == user.FirstNameEng && testUser.LastNameEng == user.LastNameEng && testUser.ProfilePicUrl == user.ProfilePicUrl);
    }

    [Fact, TestPriority(2)]
    public void Test3_GetUserPublicInfoShouldWork()
    {
        var userPublicInfo = _usersService.GetUserPublicInfo(testUser.UserId).Result;
        
        Assert.NotNull(userPublicInfo);
        Assert.True(testUser.FirstNameEng == userPublicInfo.FirstNameEng && testUser.LastNameEng == userPublicInfo.LastNameEng && testUser.ProfilePicUrl == userPublicInfo.ProfilePicUrl);
    }

    [Fact, TestPriority(10)] 
    public void Test4_DeleteUserShouldWork()
    {
        _usersService.DeleteUser(testUser.UserId).Wait();
        User? user = _usersService.GetUserByEmail(testUser.Email).Result;
        
        Assert.Null(user);
    }
}
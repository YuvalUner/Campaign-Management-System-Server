using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

/// <summary>
/// A collection of tests for the <see cref="IVerificationCodeService"/> interface and its implementation, <see cref="VerificationCodeService"/>.<br/>
/// The tests are executed in a sequential order, as defined by the <see cref="PriorityOrderer"/>,
/// using the <see cref="TestPriorityAttribute"/> attribute.
/// </summary>
[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class VerificationCodeServiceTests
{
    private readonly IVerificationCodeService _verificationCodeService;
    private readonly IConfiguration _configuration;
    private readonly IUsersService _usersService;

    private static User testUser = new User()
    {
        UserId = 53,
        Email = "bbb"
    };

    private static PhoneVerificationCode testVerificationCode = new()
    {
        PhoneNumber = "1234567890",
        VerificationCode = "123456",
    };

    public VerificationCodeServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        _verificationCodeService = new VerificationCodeService(new GenericDbAccess(_configuration));
        _usersService = new UsersService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(0)]
    public void GetVerificationCodeShouldReturnNull()
    {
        // Arrange

        // Act
        var result = _verificationCodeService.GetVerificationCode(testUser.UserId).Result;

        // Assert
        Assert.Null(result);
    }

    [Fact, TestPriority(0)]
    public void GetUserPhoneNumberShouldReturnNull()
    {
        // Arrange

        // Act
        var result = _usersService.GetUserContactInfo(testUser.UserId).Result;

        // Assert
        Assert.Null(result.PhoneNumber);
    }

    [Fact, TestPriority(1)]
    public void CreateVerificationCodeShouldCreateCode()
    {
        // Arrange

        // Act
        _verificationCodeService.CreateVerificationCode(testUser.UserId, testVerificationCode).Wait();

        // Assert
        var result = _verificationCodeService.GetVerificationCode(testUser.UserId).Result;
        Assert.NotNull(result);
        Assert.Equal(testVerificationCode.PhoneNumber, result.PhoneNumber);
        Assert.Equal(testVerificationCode.VerificationCode, result.VerificationCode);
    }

    [Fact, TestPriority(2)]
    public void ApproveVerificationCodeShouldApproveCode()
    {
        // Arrange

        // Act
        _verificationCodeService.ApproveVerificationCode(testUser.UserId, testVerificationCode.PhoneNumber).Wait();

        // Assert
        var result = _verificationCodeService.GetVerificationCode(testUser.UserId).Result;
        Assert.Null(result);
    }

    [Fact, TestPriority(3)]
    public void GetUserPhoneNumberShouldReturnPhoneNumber()
    {
        // Arrange

        // Act
        var result = _usersService.GetUserContactInfo(testUser.UserId).Result;

        // Assert
        Assert.Equal(testVerificationCode.PhoneNumber, result.PhoneNumber);
    }

    [Fact, TestPriority(100)]
    public void RemovePhoneNumberShouldDeletePhoneNumber()
    {
        // Arrange

        // Act
        _usersService.RemovePhoneNumber(testUser.UserId).Wait();

        // Assert
        var result = _usersService.GetUserContactInfo(testUser.UserId).Result;
        Assert.Null(result.PhoneNumber);
    }
}
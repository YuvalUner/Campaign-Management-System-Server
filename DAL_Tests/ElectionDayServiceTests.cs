using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class ElectionDayServiceTests
{
    private readonly IElectionDayService _electionDayService;
    private readonly IConfiguration _configuration;

    private static User _testUser = new User()
    {
        UserId = 1
    };

    private static Ballot _testBallot = new Ballot()
    {
        InnerCityBallotId = Decimal.Parse("57.3"),
        CityName = "הוד השרון",
        BallotAddress = "הדרים,12 ",
        BallotLocation = """בי"ס ממלכתי רעות""",
        Accessible = false
    };
    
    public ElectionDayServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _electionDayService = new ElectionDayService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(1)]
    public void GetBallotForUser_ShouldWorkForValidUserId()
    {
        // Arrange
        
        // Act
        var result = _electionDayService.GetUserAssignedBallot(_testUser.UserId).Result;
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(_testBallot.InnerCityBallotId, result?.InnerCityBallotId);
        Assert.Equal(_testBallot.CityName, result?.CityName);
        Assert.Equal(_testBallot.BallotAddress, result?.BallotAddress);
        Assert.Equal(_testBallot.BallotLocation, result?.BallotLocation);
        Assert.Equal(_testBallot.Accessible, result?.Accessible);
    }
    
    [Fact, TestPriority(1)]
    public void GetBallotForUser_ShouldReturnNullForInvalidUserId()
    {
        // Arrange
        
        // Act
        var result = _electionDayService.GetUserAssignedBallot(-1).Result;
        
        // Assert
        Assert.Null(result);
    }
}
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

/// <summary>
/// A collection of tests for the <see cref="INotificationsService"/> interface and its implementation, <see cref="NotificationsService"/>.<br/>
/// The tests are executed in a sequential order, as defined by the <see cref="PriorityOrderer"/>,
/// using the <see cref="TestPriorityAttribute"/> attribute.
/// </summary>
[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class VotersLedgerServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IVotersLedgerService _votersLedgerService;
    private readonly ICampaignsService _campaignsService;

    private static Campaign testCampaign = new()
    {
        CampaignName = "Test Campaign",
        CampaignDescription = "Test Campaign Description",
        CityName = "גבעתיים",
        IsMunicipal = true
    };

    private static User testUser = new()
    {
        UserId = 53,
        Email = "bbb"
    };

    private static VotersLedgerRecord testVotersLedgerRecord = new()
    {
        IdNum = 1,
        FirstName = "Test",
        LastName = "User",
        FathersName = "Erasmus",
        CityId = 6300,
        BallotId = 8636,
        ResidenceId = 6300,
        ResidenceName = "גבעתיים",
        StreetId = 602,
        StreetName = "אפק",
        HouseNumber = 207,
        Appartment = 59,
        ZipCode = 38299
    };

    private static Decimal innerCityBallotId = Decimal.Parse("72.1");

    public VotersLedgerServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        _votersLedgerService = new VotersLedgerService(new GenericDbAccess(_configuration));
        _campaignsService = new CampaignsService(new GenericDbAccess(_configuration));
    }

    /// <summary>
    /// Not an actual test, just a setup for the other tests.
    /// </summary>
    [Fact, TestPriority(0)]
    public void TestSetup()
    {
        // Arrange

        // Act
        var result = _campaignsService.AddCampaign(testCampaign, testUser.UserId).Result;
        testCampaign.CampaignId = result;
        testCampaign.CampaignGuid = _campaignsService.GetCampaignGuid(testCampaign.CampaignId).Result;

        // Assert
        Assert.True(result > 0);
    }

    [Fact, TestPriority(1)]
    public void GetVotersLedgerRecordShouldReturnNullForNonExistingId()
    {
        // Arrange

        // Act
        var result = _votersLedgerService.GetSingleVotersLedgerRecord(-1).Result;

        // Assert
        Assert.Null(result);
    }

    [Fact, TestPriority(1)]
    public void GetVotersLedgerRecordShouldReturnRecord()
    {
        // Arrange

        // Act
        var result = _votersLedgerService.GetSingleVotersLedgerRecord(testVotersLedgerRecord.IdNum).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testVotersLedgerRecord.IdNum, result.IdNum);
        Assert.Equal(testVotersLedgerRecord.FirstName, result.FirstName);
        Assert.Equal(testVotersLedgerRecord.LastName, result.LastName);
        Assert.Equal(testVotersLedgerRecord.FathersName, result.FathersName);
        Assert.Equal(testVotersLedgerRecord.CityId, result.CityId);
        Assert.Equal(testVotersLedgerRecord.BallotId, result.BallotId);
        Assert.Equal(testVotersLedgerRecord.ResidenceId, result.ResidenceId);
        Assert.Equal(testVotersLedgerRecord.ResidenceName, result.ResidenceName);
        Assert.Equal(testVotersLedgerRecord.StreetId, result.StreetId);
        Assert.Equal(testVotersLedgerRecord.StreetName, result.StreetName);
        Assert.Equal(testVotersLedgerRecord.HouseNumber, result.HouseNumber);
        Assert.Equal(testVotersLedgerRecord.Appartment, result.Appartment);
        Assert.Equal(testVotersLedgerRecord.ZipCode, result.ZipCode);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerByCityRecordShouldReturnMany()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 100);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerByCityAndIdRecordShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            IdNum = 999999999,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerByCityAndIdRecordShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            IdNum = testVotersLedgerRecord.IdNum,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerByCityAndNameRecordShouldReturnAtLeastOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = testVotersLedgerRecord.FirstName,
            LastName = testVotersLedgerRecord.LastName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerByCityAndNameRecordShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = "testVotersLedgerRecord.FirstName",
            LastName = "testVotersLedgerRecord.LastName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusShouldReturnNone()
    {
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            SupportStatus = true,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsShouldFailForNoCityName()
    {
        var filter = new VotersLedgerFilter()
        {
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsShouldThrowErrorForNoCampaignGuid()
    {
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act

        // Assert
        Assert.Throws<AggregateException>(() =>
            _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList());
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordByCityNameAndStreetNameShouldReturnMultiple()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            StreetName = testVotersLedgerRecord.StreetName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordByCityNameAndStreetNameShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            StreetName = "אפק2",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameAndFirstNameShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = testVotersLedgerRecord.FirstName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameAndFirstNameShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = "testVotersLedgerRecord.FirstName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameAndLastNameShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            LastName = testVotersLedgerRecord.LastName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameAndLastNameShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            LastName = "testVotersLedgerRecord.LastName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByIdNumShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            IdNum = testVotersLedgerRecord.IdNum,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByIdNumShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            IdNum = -1,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByFirstNameAndLastNameShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            FirstName = testVotersLedgerRecord.FirstName,
            LastName = testVotersLedgerRecord.LastName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByFirstNameAndLastNameShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            FirstName = "testVotersLedgerRecord.FirstName",
            LastName = "testVotersLedgerRecord.LastName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByFirstNameShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            FirstName = testVotersLedgerRecord.FirstName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByFirstNameShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            FirstName = "testVotersLedgerRecord.FirstName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByLastNameShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            LastName = testVotersLedgerRecord.LastName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByLastNameShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            LastName = "testVotersLedgerRecord.LastName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameBallotIdStreetNameShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            BallotId = innerCityBallotId,
            StreetName = testVotersLedgerRecord.StreetName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameBallotIdStreetNameShouldReturnNoneForFaultyStreetName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            BallotId = innerCityBallotId,
            StreetName = "testVotersLedgerRecord.StreetName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameBallotIdStreetNameShouldReturnNoneForFaultyBallotId()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            BallotId = -10,
            StreetName = testVotersLedgerRecord.StreetName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityNameBallotIdStreetNameShouldReturnNoneForFaultyCityName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = "testVotersLedgerRecord.ResidenceName",
            BallotId = innerCityBallotId,
            StreetName = testVotersLedgerRecord.StreetName,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    public void GetFilteredVotersLedgerRecordsByCityNameShouldReturnNoneForFaultyCityName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = "testVotersLedgerRecord.ResidenceName",
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndFirstNameAndBallotIdShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = testVotersLedgerRecord.FirstName,
            BallotId = innerCityBallotId,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndFirstNameAndBallotIdShouldReturnNoneForFaultyFirstName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = "testVotersLedgerRecord.FirstName",
            BallotId = innerCityBallotId,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndFirstNameAndBallotIdShouldReturnNoneForFaultyBallotId()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            FirstName = testVotersLedgerRecord.FirstName,
            BallotId = -10,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndFirstNameAndBallotIdShouldReturnNoneForFaultyCityName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = "testVotersLedgerRecord.ResidenceName",
            FirstName = testVotersLedgerRecord.FirstName,
            BallotId = innerCityBallotId,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndLastNameAndBallotIdShouldWork()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            LastName = testVotersLedgerRecord.LastName,
            BallotId = innerCityBallotId,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndLastNameAndBallotIdShouldReturnNoneForFaultyLastName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            LastName = "testVotersLedgerRecord.LastName",
            BallotId = innerCityBallotId,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndLastNameAndBallotIdShouldReturnNoneForFaultyBallotId()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = testVotersLedgerRecord.ResidenceName,
            LastName = testVotersLedgerRecord.LastName,
            BallotId = -10,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(1)]
    public void GetFilteredVotersLedgerRecordsByCityAndLastNameAndBallotIdShouldReturnNoneForFaultyCityName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            CityName = "testVotersLedgerRecord.ResidenceName",
            LastName = testVotersLedgerRecord.LastName,
            BallotId = innerCityBallotId,
            CampaignGuid = testCampaign.CampaignGuid
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(2)]
    public void UpdateSupportStatusShouldWork()
    {
        // Arrange
        var param = new UpdateSupportStatusParams()
        {
            IdNum = testVotersLedgerRecord.IdNum,
            SupportStatus = true
        };

        // Act
        var result = _votersLedgerService.UpdateVoterSupportStatus(param, testCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(2)]
    public void UpdateSupportStatusShouldReturnNotFoundForFaultyCampaignGuid()
    {
        // Arrange
        var param = new UpdateSupportStatusParams()
        {
            IdNum = testVotersLedgerRecord.IdNum,
            SupportStatus = true
        };

        // Act
        var result = _votersLedgerService.UpdateVoterSupportStatus(param, Guid.Empty).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CityNotFound, result);
    }

    [Fact, TestPriority(3)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = true,
            CampaignGuid = testCampaign.CampaignGuid,
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(3)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusShouldReturnNoneForSupportStatusFalse()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = false,
            CampaignGuid = testCampaign.CampaignGuid,
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(3)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusShouldReturnNoneForFaultyCityName()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = true,
            CampaignGuid = testCampaign.CampaignGuid,
            CityName = "testVotersLedgerRecord.ResidenceName"
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(3)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusShouldReturnNoneForFaultyCampaignGuid()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = true,
            CampaignGuid = Guid.Empty,
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(4)]
    public void UpdateVoterSupportStatusShouldWork()
    {
        // Arrange
        var param = new UpdateSupportStatusParams()
        {
            IdNum = testVotersLedgerRecord.IdNum,
            SupportStatus = false
        };

        // Act
        var result = _votersLedgerService.UpdateVoterSupportStatus(param, testCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(5)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusAfterUpdateShouldReturnNone()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = true,
            CampaignGuid = testCampaign.CampaignGuid,
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 0);
    }

    [Fact, TestPriority(5)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusAfterUpdateShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = false,
            CampaignGuid = testCampaign.CampaignGuid,
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(5)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusAndFirstNameShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = false,
            CampaignGuid = testCampaign.CampaignGuid,
            FirstName = testVotersLedgerRecord.FirstName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(5)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusAndLastNameShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = false,
            CampaignGuid = testCampaign.CampaignGuid,
            LastName = testVotersLedgerRecord.LastName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(5)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusAndFirstNameAndLastNameShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = false,
            CampaignGuid = testCampaign.CampaignGuid,
            FirstName = testVotersLedgerRecord.FirstName,
            LastName = testVotersLedgerRecord.LastName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }

    [Fact, TestPriority(5)]
    public void GetFilteredVotersLedgerRecordsBySupportStatusAndFirstNameAndLastNameAndCityNameShouldReturnOne()
    {
        // Arrange
        var filter = new VotersLedgerFilter()
        {
            SupportStatus = false,
            CampaignGuid = testCampaign.CampaignGuid,
            FirstName = testVotersLedgerRecord.FirstName,
            LastName = testVotersLedgerRecord.LastName,
            CityName = testVotersLedgerRecord.ResidenceName
        };

        // Act
        var result = _votersLedgerService.GetFilteredVotersLedgerResults(filter).Result.ToList();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
        Assert.Contains(result, x => x.IdNum == testVotersLedgerRecord.IdNum);
    }


    /// <summary>
    /// Not an actual test, just a cleanup for the other tests.
    /// What this does is tested in CampaignsServiceInvitesServiceTests.
    /// </summary>
    [Fact, TestPriority(100)]
    public void CleanUpCampaign()
    {
        // Arrange

        // Act
        _campaignsService.DeleteCampaign(testCampaign.CampaignGuid).Wait();
        var result = _campaignsService.GetCampaignNameByGuid(testCampaign.CampaignGuid).Result;

        // Assert
        Assert.Null(result);
    }
}
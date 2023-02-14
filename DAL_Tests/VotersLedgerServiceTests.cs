using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

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
        var result = _campaignsService.AddCampaign(testCampaign,testUser.UserId).Result;
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
        Assert.Contains(result, x => x.FirstName == testVotersLedgerRecord.FirstName);
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
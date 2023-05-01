using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class CustomVotersLedgerServiceTests
{
    #region setup
    
    private readonly IConfiguration _configuration;
    private readonly ICustomVotersLedgerService _customVotersLedgerService;
    
    private static Campaign _testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6"),
        CampaignId = 52
    };

    private static Campaign _testCampaignForErrorChecking = new Campaign()
    {
        CampaignGuid = Guid.Parse("050528E2-ED6A-4192-B560-2594C9ED3370")
    };

    private static CustomVotersLedger _customVotersLedger = new CustomVotersLedger()
    {
        CampaignGuid = _testCampaign.CampaignGuid,
        LedgerName = "Test Ledger"
    };

    private static CustomVotersLedgerContent _testContents = new CustomVotersLedgerContent()
    {
        Identifier = 1,
        LastName = "Test",
        FirstName = "XXXX",
        CityName = "Test",
        BallotId = 1,
        StreetName = "Test",
        HouseNumber = 1,
        Entrance = "Test",
        Appartment = "Test",
        HouseLetter = "Test",
        ZipCode = 1,
        Email1 = "Test",
        Email2 = "Test",
        Phone1 = "Test",
        Phone2 = "Test",
        SupportStatus = true
    };
    
    public CustomVotersLedgerServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _customVotersLedgerService = new CustomVotersLedgerService(new GenericDbAccess(_configuration));
    }
    
    #endregion

    [Fact, TestPriority(1)]
    public void GetCustomVotersLedgers_ShouldReturnEmptyList_ForNoLedgersYet()
    {
        // Arrange
        
        // Act
        var result = _customVotersLedgerService
            .GetCustomVotersLedgersByCampaignGuid(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(1)]
    public void GetCustomVotersLedgers_ShouldReturnEmptyList_ForNonExistingCampaignGuid()
    {
        // Arrange
        
        // Act
        var result = _customVotersLedgerService
            .GetCustomVotersLedgersByCampaignGuid(Guid.Empty).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(2)]
    public void CreateCustomVotersLedger_ShouldReturnNewLedgerGuid_ForValidData()
    {
        // Arrange
        
        // Act
        var (statusCode, guid) = _customVotersLedgerService
            .CreateCustomVotersLedger(_customVotersLedger).Result;
        
        // Assert
        Assert.NotEqual(Guid.Empty, guid);
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        
        _customVotersLedger.LedgerGuid = guid;
    }
    
    [Fact, TestPriority(2)]
    public void CreateCustomVotersLedger_ShouldReturnError_ForNonExistingCampaignGuid()
    {
        // Arrange
        var customVotersLedger = new CustomVotersLedger()
        {
            CampaignGuid = Guid.Empty,
            LedgerName = "Test Ledger"
        };
        
        // Act
        var (statusCode, guid) = _customVotersLedgerService
            .CreateCustomVotersLedger(customVotersLedger).Result;
        
        // Assert
        Assert.Equal(Guid.Empty, guid);
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
    }
    
    [Fact, TestPriority(3)]
    public void GetCustomVotersLedgers_ShouldReturnListWithOneLedger_ForValidData()
    {
        // Arrange
        
        // Act
        var result = _customVotersLedgerService
            .GetCustomVotersLedgersByCampaignGuid(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Equal(_customVotersLedger.LedgerGuid, result.First().LedgerGuid);
        Assert.Equal(_customVotersLedger.LedgerName, result.First().LedgerName);
    }
    
    [Fact, TestPriority(4)]
    public void UpdateCustomVotersLedger_ShouldReturnOk_ForValidData()
    {
        // Arrange
        _customVotersLedger.LedgerName = "Test Ledger 2";
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedger(_customVotersLedger, _testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(4)]
    public void UpdateCustomVotersLedger_ShouldReturnError_ForNonExistingLedgerGuid()
    {
        // Arrange
        var customVotersLedger = new CustomVotersLedger()
        {
            CampaignGuid = _testCampaign.CampaignGuid,
            LedgerGuid = Guid.Empty,
            LedgerName = "Test Ledger"
        };
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedger(customVotersLedger, _testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerNotFound, statusCode);
    }
    
    [Fact, TestPriority(4)]
    public void UpdateCustomVotersLedger_ShouldReturnError_ForNonExistingCampaignGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedger(_customVotersLedger, Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
    }

    [Fact, TestPriority(4)]
    public void UpdateCustomVotersLedger_ShouldReturnError_ForUpdatingForWrongCampaign()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedger(_customVotersLedger, _testCampaignForErrorChecking.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.BoundaryViolation, statusCode);
    }
    
    [Fact, TestPriority(5)]
    public void AddCustomVotersLedgerRow_ShouldWork_ForValidData()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .AddCustomVotersLedgerRow(_testContents, _customVotersLedger.LedgerGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(5)]
    public void AddCustomVotersLedgerRow_ShouldReturnError_ForNonExistingLedgerGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .AddCustomVotersLedgerRow(_testContents, Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerNotFound, statusCode);
    }
    
    [Fact, TestPriority(6)]
    public void AddCustomVotersLedgerRow_ShouldReturnError_ForDuplicateRow()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .AddCustomVotersLedgerRow(_testContents, _customVotersLedger.LedgerGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, statusCode);
    }
    
    [Fact, TestPriority(7)]
    public void FilterCustomVotersLedger_ShouldReturnOneRow_ForValidIdentifier()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            IdNum = _testContents.Identifier
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.FirstName == _testContents.FirstName);
    }
    
    [Fact, TestPriority(8)]
    public void UpdateCustomVotersLedgerRow_ShouldWork_ForValidData()
    {
        // Arrange
        _testContents.FirstName = "Test";
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedgerRow(_testContents, _customVotersLedger.LedgerGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(8)]
    public void UpdateCustomVotersLedgerRow_ShouldReturnError_ForNonExistingLedgerGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedgerRow(_testContents, Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerNotFound, statusCode);
    }
    
    [Fact, TestPriority(8)]
    public void UpdateCustomVotersLedgerRow_ShouldReturnError_ForNonExistingRow()
    {
        // Arrange
        var badData = new CustomVotersLedgerContent()
        {
            Identifier = -1
        };
        
        // Act
        var statusCode = _customVotersLedgerService
            .UpdateCustomVotersLedgerRow(badData, _customVotersLedger.LedgerGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerRowNotFound, statusCode);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForInvalidIdentifier()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            IdNum = -1
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnOne_ForValidFirstName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            FirstName = _testContents.FirstName
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.FirstName == _testContents.FirstName);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForInvalidFirstName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            FirstName = ""
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnOne_ForValidLastName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            LastName = _testContents.LastName
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.LastName == _testContents.LastName);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForInvalidLastName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            LastName = ""
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnOne_ForValidCityName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            CityName = _testContents.CityName
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.CityName == _testContents.CityName);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForInvalidCityName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            CityName = ""
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnOne_ForValidStreetName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            StreetName = _testContents.StreetName
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.StreetName == _testContents.StreetName);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForInvalidStreetName()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            StreetName = ""
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnOne_ForValidBallotId()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            BallotId = _testContents.BallotId
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.BallotId == _testContents.BallotId);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForInvalidBallotId()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            BallotId = -1
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnOne_ForCorrectSupportStatus()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            SupportStatus = _testContents.SupportStatus
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Single(result);
        Assert.Contains(result, x => x.Identifier == _testContents.Identifier && x.SupportStatus == _testContents.SupportStatus);
    }
    
    [Fact, TestPriority(9)]
    public void FilterCustomVotersLedger_ShouldReturnEmptyList_ForIncorrectSupportStatus()
    {
        // Arrange
        var filter = new CustomLedgerFilterParams()
        {
            SupportStatus = !_testContents.SupportStatus
        };
        
        // Act
        var result = _customVotersLedgerService
            .FilterCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, filter).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(10)]
    public void DeleteCustomVotersLedgerRow_ShouldReturnError_ForNonExistingLedgerGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedgerRow(Guid.Empty, _testContents.Identifier.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerNotFound, statusCode);
    }
    
    [Fact, TestPriority(10)]
    public void DeleteCustomVotersLedgerRow_ShouldReturnError_ForNonExistingIdentifier()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedgerRow(_customVotersLedger.LedgerGuid.Value, -1).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerRowNotFound, statusCode);
    }

    [Fact, TestPriority(90)]
    public void DeleteCustomVotersLedgerRow_ShouldReturnOk_ForValidParameters()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedgerRow(_customVotersLedger.LedgerGuid.Value, _testContents.Identifier.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    
    [Fact, TestPriority(100)]
    public void DeleteCustomVotersLedger_ShouldReturnOk_ForValidData()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, _testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteCustomVotersLedger_ShouldReturnError_ForNonExistingLedgerGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedger(Guid.Empty, _testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.LedgerNotFound, statusCode);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteCustomVotersLedger_ShouldReturnError_ForNonExistingCampaignGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteCustomVotersLedger_ShouldReturnError_ForDeletingFromWrongCampaign()
    {
        // Arrange
        
        // Act
        var statusCode = _customVotersLedgerService
            .DeleteCustomVotersLedger(_customVotersLedger.LedgerGuid.Value, _testCampaignForErrorChecking.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.BoundaryViolation, statusCode);
    }
    
    [Fact, TestPriority(101)]
    public void GetCustomVotersLedgers_ShouldReturnEmptyList_ForDeletedLedger()
    {
        // Arrange
        
        // Act
        var result = _customVotersLedgerService
            .GetCustomVotersLedgersByCampaignGuid(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Empty(result);
    }
}
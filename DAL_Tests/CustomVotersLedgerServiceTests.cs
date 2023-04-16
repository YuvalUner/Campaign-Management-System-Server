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
    
    public CustomVotersLedgerServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _customVotersLedgerService = new CustomVotersLedgerService(new GenericDbAccess(_configuration));
    }

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
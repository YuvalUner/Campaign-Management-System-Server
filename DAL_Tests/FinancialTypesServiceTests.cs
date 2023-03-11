using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class FinancialTypesServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IFinancialTypesService _financialTypesService;

    private static FinancialType _testFinancialType = new FinancialType()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6"),
        TypeName = "Test",
        TypeDescription = "Test"
    };

    private static FinancialType _otherFinancialType = new FinancialType()
    {
        TypeName = "Other",
        TypeGuid = Guid.Parse("F082354C-7B51-4687-AC26-D833EB301556")
    };
    
    public FinancialTypesServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _financialTypesService = new FinancialTypesService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(1)]
    public void GetFinancialTypes_ShouldReturnListWithOnlyOtherType_ForNoTypesAddedYet()
    {
        // Arrange
        
        // Act
        var financialTypes = _financialTypesService.GetFinancialTypes(_testFinancialType.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Single(financialTypes);
        Assert.Equal(_otherFinancialType.TypeGuid, financialTypes.First().TypeGuid);
    }
    
    [Fact, TestPriority(2)]
    public void CreateFinancialType_ShouldWork()
    {
        // Arrange
        
        // Act
        var (statusCode, typeGuid) = _financialTypesService.CreateFinancialType(_testFinancialType).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        Assert.NotEqual(Guid.Empty, typeGuid);
        _testFinancialType.TypeGuid = typeGuid;
    }
    
    [Fact, TestPriority(3)]
    public void GetFinancialTypes_ShouldReturnListWithOtherAndTestTypes()
    {
        // Arrange
        
        // Act
        var financialTypes = _financialTypesService.GetFinancialTypes(_testFinancialType.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Equal(2, financialTypes.Count());
        Assert.Contains(financialTypes, ft => ft.TypeGuid == _otherFinancialType.TypeGuid);
        Assert.Contains(financialTypes, ft => ft.TypeGuid == _testFinancialType.TypeGuid);
    }

    [Fact, TestPriority(4)]
    public void UpdateFinancialType_ShouldWork()
    {
        // Arrange
        var updatedFinancialType = new FinancialType()
        {
            TypeGuid = _testFinancialType.TypeGuid,
            TypeName = "Updated",
            TypeDescription = "Updated",
            CampaignGuid = _testFinancialType.CampaignGuid
        };
        
        // Act
        var statusCode = _financialTypesService.UpdateFinancialType(updatedFinancialType).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
        _testFinancialType = updatedFinancialType;
    }
    
    [Fact, TestPriority(5)]
    public void GetFinancialTypes_ShouldReturnListWithUpdatedTestType()
    {
        // Arrange
        
        // Act
        var financialTypes = _financialTypesService.GetFinancialTypes(_testFinancialType.CampaignGuid.Value).Result;
        
        // Assert
        Assert.NotEmpty(financialTypes);
        Assert.Contains(financialTypes, ft => ft.TypeGuid == _otherFinancialType.TypeGuid);
        Assert.Contains(financialTypes, ft => ft.TypeGuid == _testFinancialType.TypeGuid);
        Assert.Contains(financialTypes, ft => ft.TypeName == _testFinancialType.TypeName);
        Assert.Contains(financialTypes, ft => ft.TypeDescription == _testFinancialType.TypeDescription);
    }

    [Fact, TestPriority(6)]
    public void AddFinancialType_ShouldFail_ForWrongCampaignGuid()
    {
        // Arrange
        var financialType = new FinancialType()
        {
            CampaignGuid = Guid.Empty,
            TypeName = "Test",
            TypeDescription = "Test"
        };
        
        // Act
        var (statusCode, typeGuid) = _financialTypesService.CreateFinancialType(financialType).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
        Assert.Equal(Guid.Empty, typeGuid);
    }
    
    [Fact, TestPriority(6)]
    public void UpdateFinancialType_ShouldFail_ForWrongTypeGuid()
    {
        // Arrange
        var financialType = new FinancialType()
        {
            TypeGuid = Guid.Empty,
            TypeName = "Test",
            TypeDescription = "Test"
        };
        
        // Act
        var statusCode = _financialTypesService.UpdateFinancialType(financialType).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.FinancialTypeNotFound, statusCode);
    }
    
    [Fact, TestPriority(6)]
    public void DeleteFinancialType_ShouldFail_ForWrongTypeGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _financialTypesService.DeleteFinancialType(Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.FinancialTypeNotFound, statusCode);
    }

    [Fact, TestPriority(6)]
    public void UpdateFinancialType_ShouldFail_ForIllegalTypeGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _financialTypesService.UpdateFinancialType(_otherFinancialType).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.SqlIllegalValue, statusCode);
    }
    
    [Fact, TestPriority(6)]
    public void DeleteFinancialType_ShouldFail_ForIllegalTypeGuid()
    {
        // Arrange
        
        // Act
        var statusCode = _financialTypesService.DeleteFinancialType(_otherFinancialType.TypeGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.SqlIllegalValue, statusCode);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteFinancialType_ShouldWork()
    {
        // Arrange
        
        // Act
        var statusCode = _financialTypesService.DeleteFinancialType(_testFinancialType.TypeGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, statusCode);
    }
    
    [Fact, TestPriority(101)]
    public void GetFinancialTypes_ShouldReturnListWithOnlyOtherType_AfterDelete()
    {
        // Arrange
        
        // Act
        var financialTypes = _financialTypesService.GetFinancialTypes(_testFinancialType.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Single(financialTypes);
        Assert.Equal(_otherFinancialType.TypeGuid, financialTypes.First().TypeGuid);
    }
}
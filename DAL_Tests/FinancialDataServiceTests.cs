using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class FinancialDataServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IFinancialDataService _financialDataService;

    private static User _testUser = new User()
    {
        UserId = 53,
        Email = "bbb"
    };

    private static Campaign _testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6")
    };
    
    private static FinancialType _testFinancialType = new FinancialType()
    {
        TypeGuid = Guid.Parse("68770323-B486-4DEE-898C-2FC502312D1F")
    };

    private static FinancialType _other = new FinancialType()
    {
        TypeGuid = Guid.Parse("F082354C-7B51-4687-AC26-D833EB301556")
    };

    private static FinancialDataEntry _testFinancialDataEntryTestFinancialTypeExpense = new FinancialDataEntry()
    {
        Amount = 50,
        CampaignGuid = _testCampaign.CampaignGuid,
        CreatorUserId = _testUser.UserId,
        DataDescription = "Test",
        DataTitle = "Test",
        TypeGuid = _testFinancialType.TypeGuid,
        DateCreated = DateTime.Parse("2021-01-01"),
        IsExpense = true
    };
    
    private static FinancialDataEntry _testFinancialDataEntryTestFinancialTypeIncome = new FinancialDataEntry()
    {
        Amount = 50,
        CampaignGuid = _testCampaign.CampaignGuid,
        CreatorUserId = _testUser.UserId,
        DataDescription = "Test",
        DataTitle = "Test",
        TypeGuid = _testFinancialType.TypeGuid,
        DateCreated = DateTime.Parse("2021-01-01"),
        IsExpense = false
    };
    
    private static FinancialDataEntry _testFinancialDataEntryOtherFinancialTypeExpense = new FinancialDataEntry()
    {
        Amount = 50,
        CampaignGuid = _testCampaign.CampaignGuid,
        CreatorUserId = _testUser.UserId,
        DataDescription = "Test",
        DataTitle = "Test",
        TypeGuid = _other.TypeGuid,
        DateCreated = DateTime.Parse("2021-01-01"),
        IsExpense = true
    };
    
    private static FinancialDataEntry _testFinancialDataEntryOtherFinancialTypeIncome = new FinancialDataEntry()
    {
        Amount = 50,
        CampaignGuid = _testCampaign.CampaignGuid,
        CreatorUserId = _testUser.UserId,
        DataDescription = "Test",
        DataTitle = "Test",
        TypeGuid = _other.TypeGuid,
        DateCreated = DateTime.Parse("2021-01-01"),
        IsExpense = false
    };
    
    
    
    public FinancialDataServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _financialDataService = new FinancialDataService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(1)]
    public void GetFinancialSummary_ShouldReturnEmpty_ForNoDataYet()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialSummary(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(1)]
    public void GetFinancialDataForCampaign_ShouldReturnEmpty_ForNoDataYet()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialDataForCampaign(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldReturnSuccess_ForValidData1()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.AddFinancialDataEntry(_testFinancialDataEntryTestFinancialTypeExpense).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result.Item1);
        Assert.NotEqual(Guid.Empty, result.Item2);
        _testFinancialDataEntryTestFinancialTypeExpense.DataGuid = result.Item2;
    }
    
    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldReturnSuccess_ForValidData2()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.AddFinancialDataEntry(_testFinancialDataEntryTestFinancialTypeIncome).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result.Item1);
        Assert.NotEqual(Guid.Empty, result.Item2);
        _testFinancialDataEntryTestFinancialTypeIncome.DataGuid = result.Item2;
    }
    
    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldReturnSuccess_ForValidData3()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.AddFinancialDataEntry(_testFinancialDataEntryOtherFinancialTypeExpense).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result.Item1);
        Assert.NotEqual(Guid.Empty, result.Item2);
        _testFinancialDataEntryOtherFinancialTypeExpense.DataGuid = result.Item2;
    }
    
    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldReturnSuccess_ForValidData4()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.AddFinancialDataEntry(_testFinancialDataEntryOtherFinancialTypeIncome).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result.Item1);
        Assert.NotEqual(Guid.Empty, result.Item2);
        _testFinancialDataEntryOtherFinancialTypeIncome.DataGuid = result.Item2;
    }

    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldFail_ForWrongCampaignGuid()
    {
        // Arrange
        var invalidFinancialData = new FinancialDataEntry()
        {
            Amount = 50,
            CampaignGuid = Guid.Empty,
            CreatorUserId = _testUser.UserId,
            DataDescription = "Test",
            DataTitle = "Test",
            TypeGuid = _testFinancialType.TypeGuid,
            DateCreated = DateTime.Parse("2021-01-01"),
            IsExpense = true
        };
        
        // Act
        var (statusCode, newGuid) = _financialDataService.AddFinancialDataEntry(invalidFinancialData).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.CampaignNotFound, statusCode);
        Assert.Equal(Guid.Empty, newGuid);
    }
    
    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldFail_ForWrongFinancialTypeGuid()
    {
        // Arrange
        var invalidFinancialData = new FinancialDataEntry()
        {
            Amount = 50,
            CampaignGuid = _testCampaign.CampaignGuid,
            CreatorUserId = _testUser.UserId,
            DataDescription = "Test",
            DataTitle = "Test",
            TypeGuid = Guid.Empty,
            DateCreated = DateTime.Parse("2021-01-01"),
            IsExpense = true
        };
        
        // Act
        var (statusCode, newGuid) = _financialDataService.AddFinancialDataEntry(invalidFinancialData).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.FinancialTypeNotFound, statusCode);
        Assert.Equal(Guid.Empty, newGuid);
    }
    
    [Fact, TestPriority(2)]
    public void AddFinancialDataEntry_ShouldFail_ForWrongUserId()
    {
        // Arrange
        var invalidFinancialData = new FinancialDataEntry()
        {
            Amount = 50,
            CampaignGuid = _testCampaign.CampaignGuid,
            CreatorUserId = -1,
            DataDescription = "Test",
            DataTitle = "Test",
            TypeGuid = _testFinancialType.TypeGuid,
            DateCreated = DateTime.Parse("2021-01-01"),
            IsExpense = true
        };
        
        // Act
        var (statusCode, newGuid) = _financialDataService.AddFinancialDataEntry(invalidFinancialData).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, statusCode);
        Assert.Equal(Guid.Empty, newGuid);
    }

    [Fact, TestPriority(3)]
    public void GetFinancialSummary_ShouldReturnCorrectData_AfterDataWasAdded()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialSummary(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.NotEmpty(result);
        
        var expenses = result.Where(x => x.IsExpense).ToList();
        var incomes = result.Where(x => !x.IsExpense).ToList();
        
        Assert.NotEmpty(expenses);
        Assert.NotEmpty(incomes);
        
        // Make sure the correct financial types are returned
        Assert.Contains(expenses, x => x.TypeGuid == _testFinancialType.TypeGuid);
        Assert.Contains(incomes, x => x.TypeGuid == _testFinancialType.TypeGuid);
        Assert.Contains(expenses, x => x.TypeGuid == _other.TypeGuid);
        Assert.Contains(incomes, x => x.TypeGuid == _other.TypeGuid);
        
        // Make sure the sum of the amounts is correct
        Assert.Equal(expenses.Sum(x => x.TotalAmount), _testFinancialDataEntryTestFinancialTypeExpense.Amount + _testFinancialDataEntryOtherFinancialTypeExpense.Amount);
        Assert.Equal(incomes.Sum(x => x.TotalAmount), _testFinancialDataEntryTestFinancialTypeIncome.Amount + _testFinancialDataEntryOtherFinancialTypeIncome.Amount);
        
        // Calculate the expected values for the balance of each financial type
        var expectedBalanceTestFinancialType = _testFinancialDataEntryTestFinancialTypeIncome.Amount - _testFinancialDataEntryTestFinancialTypeExpense.Amount;
        var expectedBalanceOtherFinancialType = _testFinancialDataEntryOtherFinancialTypeIncome.Amount - _testFinancialDataEntryOtherFinancialTypeExpense.Amount;
        
        // Make sure the balance is correct
        foreach (var income in incomes)
        {
            var expense = expenses.First(x => income.TypeGuid == x.TypeGuid);
            if (income.TypeGuid == _testFinancialType.TypeGuid)
            {
                Assert.Equal(expectedBalanceTestFinancialType, income.TotalAmount - expense.TotalAmount);
            }
            else
            {
                Assert.Equal(expectedBalanceOtherFinancialType, income.TotalAmount - expense.TotalAmount);
            }
        }
    }
    
    [Fact, TestPriority(3)]
    public void GetFinancialSummary_ShouldReturnEmptyList_ForNonExistingCampaign()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialSummary(Guid.Empty).Result;
        
        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(3)]
    public void GetFinancialDataForCampaign_ShouldReturnAllDataData_ForExistingCampaign()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialDataForCampaign(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryTestFinancialTypeExpense.DataGuid);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryTestFinancialTypeIncome.DataGuid);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryOtherFinancialTypeExpense.DataGuid);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryOtherFinancialTypeIncome.DataGuid);
    }
    
    [Fact, TestPriority(3)]
    public void GetFinancialDataForCampaign_ShouldReturnEmptyList_ForNonExistingCampaign()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialDataForCampaign(Guid.Empty).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(3)]
    public void GetFinancialDataForCampaign_ShouldReturnEmptyList_ForNonExistingFinancialType()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialDataForCampaign(_testCampaign.CampaignGuid.Value, Guid.Empty).Result;
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact, TestPriority(3)]
    public void GetFinancialDataForCampaign_ShouldReturnData_OnlyForExistingFinancialType()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialDataForCampaign(_testCampaign.CampaignGuid.Value, _testFinancialType.TypeGuid.Value).Result;
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryTestFinancialTypeExpense.DataGuid);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryTestFinancialTypeIncome.DataGuid);
        Assert.DoesNotContain(result, x => x.TypeGuid == _testFinancialDataEntryOtherFinancialTypeExpense.TypeGuid);
    }
    
    [Fact, TestPriority(4)]
    public void UpdateFinancialDataEntry_ShouldReturnSuccess_ForValidData()
    {
        // Arrange
        var updatedFinancialData = new FinancialDataEntry
        {
            DataGuid = _testFinancialDataEntryTestFinancialTypeExpense.DataGuid,
            CampaignGuid = _testCampaign.CampaignGuid,
            CreatorUserId = _testUser.UserId,
            DataDescription = "Test",
            DataTitle = "Test",
            TypeGuid = _testFinancialType.TypeGuid,
            DateCreated = DateTime.Parse("2021-01-01"),
            IsExpense = true,
            Amount = 100,
        };
        
        // Act
        var result = _financialDataService.UpdateFinancialDataEntry(updatedFinancialData).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
        _testFinancialDataEntryTestFinancialTypeExpense = updatedFinancialData;
    }
    
    [Fact, TestPriority(4)]
    public void UpdateFinancialDataEntry_ShouldReturnError_ForNonExistingData()
    {
        // Arrange
        var updatedFinancialData = new FinancialDataEntry
        {
            DataGuid = Guid.Empty,
            CampaignGuid = _testCampaign.CampaignGuid,
            CreatorUserId = _testUser.UserId,
            DataDescription = "Test",
            DataTitle = "Test",
            TypeGuid = _testFinancialType.TypeGuid,
            DateCreated = DateTime.Parse("2021-01-01"),
            IsExpense = true,
            Amount = 100,
        };
        
        // Act
        var result = _financialDataService.UpdateFinancialDataEntry(updatedFinancialData).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.FinancialDataNotFound, result);
    }

    [Fact, TestPriority(4)]
    public void UpdateFinancialDataEntry_ShouldReturnError_ForNonExistingFinancialType()
    {
        // Arrange
        var updatedFinancialData = new FinancialDataEntry
        {
            DataGuid = _testFinancialDataEntryTestFinancialTypeExpense.DataGuid,
            CampaignGuid = _testCampaign.CampaignGuid,
            CreatorUserId = _testUser.UserId,
            DataDescription = "Test",
            DataTitle = "Test",
            TypeGuid = Guid.Empty,
            DateCreated = DateTime.Parse("2021-01-01"),
            IsExpense = true,
            Amount = 100,
        };
        
        // Act
        var result = _financialDataService.UpdateFinancialDataEntry(updatedFinancialData).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.FinancialTypeNotFound, result);
    }

    [Fact, TestPriority(5)]
    public void GetFinancialDataAfterUpdate_ShouldReturnList()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.GetFinancialDataForCampaign(_testCampaign.CampaignGuid.Value).Result;
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.DataGuid == _testFinancialDataEntryTestFinancialTypeExpense.DataGuid &&
                                     x.Amount == _testFinancialDataEntryTestFinancialTypeExpense.Amount);
    }
    
    
    [Fact, TestPriority(100)]
    public void DeleteFinancialDataEntry_ShouldReturnSuccess_ForValidData1()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.DeleteFinancialDataEntry(_testFinancialDataEntryTestFinancialTypeExpense.DataGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteFinancialDataEntry_ShouldReturnSuccess_ForValidData2()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.DeleteFinancialDataEntry(_testFinancialDataEntryTestFinancialTypeIncome.DataGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteFinancialDataEntry_ShouldReturnSuccess_ForValidData3()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.DeleteFinancialDataEntry(_testFinancialDataEntryOtherFinancialTypeExpense.DataGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteFinancialDataEntry_ShouldReturnSuccess_ForValidData4()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.DeleteFinancialDataEntry(_testFinancialDataEntryOtherFinancialTypeIncome.DataGuid.Value).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }
    
    [Fact, TestPriority(100)]
    public void DeleteFinancialDataEntry_ShouldFail_ForWrongGuid()
    {
        // Arrange
        
        // Act
        var result = _financialDataService.DeleteFinancialDataEntry(Guid.Empty).Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.FinancialDataNotFound, result);
    }
}
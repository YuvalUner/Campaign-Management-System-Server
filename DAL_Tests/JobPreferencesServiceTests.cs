using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DAL_Tests;

/// <summary>
/// A collection of tests for the <see cref="IJobPreferencesService"/> and its implementation <see cref="JobPreferencesService"/>.<br/>
/// The tests are executed in a sequential order, as defined by the <see cref="PriorityOrderer"/>,
/// using the <see cref="TestPriorityAttribute"/> attribute.
/// </summary>
[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class JobPreferencesServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IJobPreferencesService _jobPreferencesService;

    private static User testUser = new User()
    {
        UserId = 53,
        Email = "bbb",
    };

    private static Campaign testCampaign = new Campaign()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6")
    };

    private static UserJobPreference testUserJobPreference = new UserJobPreference()
    {
        UserPreferencesText = "Test"
    };

    public JobPreferencesServiceTests()
    {
        _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        _jobPreferencesService = new JobPreferencesService(new GenericDbAccess(_configuration));
    }

    [Fact, TestPriority(0)]
    public void GetUserPreferencesShouldReturnNull()
    {
        // Arrange

        // Act
        var userPreferences = _jobPreferencesService
            .GetUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Null(userPreferences);
    }

    [Fact, TestPriority(1)]
    public void AddUserPreferencesShouldWork()
    {
        // Arrange

        // Act
        _jobPreferencesService.AddUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value,
            testUserJobPreference.UserPreferencesText).Wait();

        // Assert
        var userPreferences = _jobPreferencesService
            .GetUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value).Result;
        Assert.NotNull(userPreferences);
        Assert.Equal(testUserJobPreference.UserPreferencesText, userPreferences.UserPreferencesText);
    }

    [Fact, TestPriority(2)]
    public void UpdateUserPreferencesShouldWork()
    {
        // Arrange
        testUserJobPreference.UserPreferencesText = "Test2";

        // Act
        _jobPreferencesService.UpdateUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value,
            testUserJobPreference.UserPreferencesText).Wait();

        // Assert
        var userPreferences = _jobPreferencesService
            .GetUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value).Result;
        Assert.NotNull(userPreferences);
        Assert.Equal(testUserJobPreference.UserPreferencesText, userPreferences.UserPreferencesText);
    }

    [Fact, TestPriority(3)]
    public void DeleteUserPreferencesShouldWork()
    {
        // Arrange

        // Act
        _jobPreferencesService.DeleteUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value).Wait();

        // Assert
        var userPreferences = _jobPreferencesService
            .GetUserPreferences(testUser.UserId, testCampaign.CampaignGuid.Value).Result;
        Assert.Null(userPreferences);
    }
}
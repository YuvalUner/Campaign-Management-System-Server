using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace DAL_Tests;

/// <summary>
/// A collection of tests for the <see cref="IJobAssignmentCapabilityService"/>, <see cref="IJobTypesService"/>,
/// <see cref="IJobTypeAssignmentCapabilityService"/>, and <see cref="IJobsService"/> interfaces and their implementations
/// <see cref="JobAssignmentCapabilityService"/>, <see cref="JobTypesService"/>, <see cref="JobTypeAssignmentCapabilityService"/>, 
/// and <see cref="JobsService"/> respectively.<br/>
/// The tests are executed in a sequential order, as defined by the <see cref="PriorityOrderer"/>,
/// using the <see cref="TestPriorityAttribute"/> attribute.
/// </summary>
[Collection("sequential")]
[TestCaseOrderer("DAL_Tests.PriorityOrderer", "DAL_Tests")]
public class JobRelatedServicesTestsTests
{
    private readonly IConfiguration _configuration;
    private readonly ITestOutputHelper _helper;
    private readonly IJobAssignmentCapabilityService _jobAssignmentCapabilityService;
    private readonly IJobsService _jobsService;
    private readonly IJobTypesService _jobTypesService;
    private readonly IJobTypeAssignmentCapabilityService _jobTypeAssignmentCapabilityService;

    private static Campaign TestCampaign = new()
    {
        CampaignGuid = Guid.Parse("AA444CB1-DA89-4D67-B15B-B1CB2E0E11E6")
    };

    private static User TestUser = new User()
    {
        UserId = 53,
        Email = "bbb",
    };


    private static JobType TestJobType = new()
    {
        JobTypeName = "Test Job Type",
        JobTypeDescription = "Test Job Type Description",
        IsCustomJobType = true
    };

    private static Job TestJob = new Job()
    {
        JobDefaultSalary = 5000,
        JobDescription = "Test Job Description",
        JobName = "Test Job Name",
        JobLocation = "abcd",
        PeopleNeeded = 500,
        JobTypeName = TestJobType.JobTypeName,
        JobStartTime = DateTime.Parse("2023-02-01 00:00:00"),
        JobEndTime = DateTime.Parse("2023-02-05 00:00:00"),
    };

    private static JobAssignmentCapabilityParams testJobAssignmentCapabilityParams = new()
    {
        UserEmail = TestUser.Email,
        JobGuid = Guid.Empty
    };

    private static JobTypeAssignmentCapabilityParams TestJobTypeAssignmentParams = new()
    {
        UserEmail = TestUser.Email,
        JobTypeName = TestJobType.JobTypeName
    };

    private static JobAssignmentParams TestJobAssignmentParams = new()
    {
        UserEmail = TestUser.Email,
    };

    public JobRelatedServicesTestsTests(ITestOutputHelper helper)
    {
        _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        _jobAssignmentCapabilityService = new JobAssignmentCapabilityService(new GenericDbAccess(_configuration));
        _jobsService = new JobsService(new GenericDbAccess(_configuration));
        _jobTypesService = new JobTypesService(new GenericDbAccess(_configuration));
        _jobTypeAssignmentCapabilityService =
            new JobTypeAssignmentCapabilityService(new GenericDbAccess(_configuration));
        _helper = helper;
    }

    [Fact, TestPriority(0)]
    public void GetJobTypesBeforeAddingShouldWork()
    {
        // Arrange

        // Act
        var result = _jobTypesService.GetJobTypes(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        // Even without adding any values, there should be the built in job types.
        Assert.NotEmpty(result);
        Assert.DoesNotContain(result, x => x.JobTypeName == "Test Job Type");
    }

    [Fact, TestPriority(1)]
    public void CreateJobTypeShouldWork()
    {
        // Arrange

        // Act
        var result = _jobTypesService.AddJobType(TestJobType, TestCampaign.CampaignGuid.Value, TestUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(1)]
    public void DeleteJobTypeAssignmentCapabilityShouldWork()
    {
        // Arrange
        TestJobTypeAssignmentParams.JobTypeName = TestJobType.JobTypeName;

        // Act
        _jobTypeAssignmentCapabilityService
            .RemoveJobTypeAssignmentCapableUser(TestCampaign.CampaignGuid.Value, TestJobTypeAssignmentParams).Wait();
        var result = _jobTypeAssignmentCapabilityService
            .GetJobTypeAssignmentCapableUsers(TestCampaign.CampaignGuid.Value, TestJobTypeAssignmentParams.JobTypeName)
            .Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(2)]
    public void GetJobTypesShouldWork()
    {
        // Arrange

        // Act
        var result = _jobTypesService.GetJobTypes(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobTypeName == TestJobType.JobTypeName);
    }

    [Fact, TestPriority(2)]
    public void GetJobTypesShouldFail()
    {
        // Arrange

        // Act
        var result = _jobTypesService.GetJobTypes(Guid.Empty).Result;

        // Assert
        Assert.DoesNotContain(result, x => x.JobTypeName == TestJobType.JobTypeName);
    }

    [Fact, TestPriority(2)]
    public void CreateJobTypeShouldFailForDuplicateUniqueIndex()
    {
        // Arrange

        // Act
        var result = _jobTypesService.AddJobType(TestJobType, TestCampaign.CampaignGuid.Value, TestUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.CannotInsertDuplicateUniqueIndex, result);
    }

    [Fact, TestPriority(3)]
    public void UpdateJobTypeShouldWork()
    {
        TestJobType = new JobType()
        {
            JobTypeName = "Test Job Type updated",
            JobTypeDescription = "Test Job Type Description updated",
            IsCustomJobType = true
        };

        // Act
        var result = _jobTypesService.UpdateJobType(TestJobType, TestCampaign.CampaignGuid.Value, "Test Job Type")
            .Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(4)]
    public void UpdateJobTypeShouldFailForDuplicateUniqueIndex()
    {
        // Act
        var result = _jobTypesService
            .UpdateJobType(TestJobType, TestCampaign.CampaignGuid.Value, "Test Job Type updated").Result;

        // Assert
        Assert.Equal(CustomStatusCode.CannotInsertDuplicateUniqueIndex, result);
    }

    [Fact, TestPriority(5)]
    public void GetJobTypesAfterUpdateShouldWork()
    {
        // Arrange

        // Act
        var result = _jobTypesService.GetJobTypes(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result,
            x => x.JobTypeName == TestJobType.JobTypeName && x.JobTypeDescription == TestJobType.JobTypeDescription);
    }

    [Fact, TestPriority(1)]
    public void GetJobsBeforeAddingShouldBeEmpty()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJobs(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(2)]
    public void CreateJobShouldWork()
    {
        // Arrange

        // Act
        TestJob.JobGuid = _jobsService.AddJob(TestJob, TestCampaign.CampaignGuid.Value, TestUser.UserId).Result;

        // Assert
        Assert.NotEqual(Guid.Empty, TestJob.JobGuid);
    }

    [Fact, TestPriority(3)]
    public void DeleteAssignmentCapabilityShouldWork()
    {
        // Arrange
        testJobAssignmentCapabilityParams.JobGuid = TestJob.JobGuid.Value;

        // Act
        _jobAssignmentCapabilityService
            .RemoveJobAssignmentCapableUser(TestCampaign.CampaignGuid.Value, testJobAssignmentCapabilityParams).Wait();
        var result = _jobAssignmentCapabilityService
            .GetJobAssignmentCapableUsers(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value, false).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(3)]
    public void GetJobShouldWork()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJob(TestJob.JobGuid.Value, TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TestJob.JobGuid, result.JobGuid);
        Assert.Equal(TestJob.JobDefaultSalary, result.JobDefaultSalary);
        Assert.Equal(TestJob.JobDescription, result.JobDescription);
        Assert.Equal(TestJob.JobName, result.JobName);
        Assert.Equal(TestJob.JobLocation, result.JobLocation);
        Assert.Equal(TestJob.PeopleNeeded, result.PeopleNeeded);
        Assert.Equal(TestJob.JobTypeName, result.JobTypeName);
    }

    [Fact, TestPriority(3)]
    public void GetJobShouldFailForWrongJobGuid()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJob(Guid.Empty, TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Null(result);
    }

    [Fact, TestPriority(3)]
    public void GetJobsShouldWork()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJobs(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(3)]
    public void GetJobShouldFailForWrongCampaign()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJob(TestJob.JobGuid.Value, Guid.Empty).Result;

        // Assert
        Assert.Null(result);
    }

    [Fact, TestPriority(4)]
    public void UpdateJobShouldWork()
    {
        // Arrange
        TestJob.JobName = "Test Job updated";
        TestJob.JobDescription = "Test Job Description updated";
        TestJob.JobLocation = "Test Job Location updated";
        TestJob.JobDefaultSalary = 100;
        TestJob.JobTypeName = TestJobType.JobTypeName;

        // Act
        _jobsService.UpdateJob(TestJob, TestCampaign.CampaignGuid.Value).Wait();
        var result = _jobsService.GetJob(TestJob.JobGuid.Value, TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Equal(TestJob.JobName, result.JobName);
        Assert.Equal(TestJob.JobDescription, result.JobDescription);
        Assert.Equal(TestJob.JobLocation, result.JobLocation);
        Assert.Equal(TestJob.JobDefaultSalary, result.JobDefaultSalary);
        Assert.Equal(TestJob.PeopleNeeded, result.PeopleNeeded);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByFilterJobNameShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobName = TestJob.JobName
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByFilterJobNameShouldBeEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobName = ""
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJobType.JobTypeName
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeShouldBeEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobTypeName = ""
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobLocationShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobLocation = TestJob.JobLocation
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobLocationShouldBeEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobLocation = ""
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByStartTimeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobStartTime = TestJob.JobStartTime,
            TimeFromStart = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByStartTimeShouldBeEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobStartTime = TestJob.JobStartTime.Value.Subtract(TimeSpan.FromDays(1)),
            TimeFromStart = false,
            TimeBeforeStart = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsFromStartTimeAfterStartTimeShouldReturnEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobStartTime = TestJob.JobStartTime.Value.Add(TimeSpan.FromDays(1)),
            TimeFromStart = true,
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsFromStartTimeBeforeStartTimeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobStartTime = TestJob.JobStartTime.Value.Subtract(TimeSpan.FromDays(1)),
            TimeFromStart = true,
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsBeforeStartTimeAfterStartTimeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobStartTime = TestJob.JobStartTime.Value.Add(TimeSpan.FromDays(1)),
            TimeBeforeStart = true,
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsBeforeEndTimeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobEndTime = TestJob.JobEndTime,
            TimeBeforeEnd = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsBeforeEndTimeShouldBeEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobEndTime = TestJob.JobEndTime.Value.Subtract(TimeSpan.FromDays(3)),
            TimeBeforeEnd = true,
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsFromEndTimeAfterEndTimeShouldBeEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            JobEndTime = TestJob.JobEndTime.Value.Add(TimeSpan.FromDays(1)),
            TimeFromEnd = true,
            TimeBeforeEnd = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsFromEndTimeBeforeEndTimeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobEndTime = TestJob.JobEndTime.Value.Subtract(TimeSpan.FromDays(1)),
            TimeFromEnd = true,
            TimeBeforeEnd = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByMannedStatusFullyMannedShouldReturnEmpty()
    {
        var filter = new JobsFilterParameters()
        {
            FullyManned = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByMannedStatusNotFullyMannedShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            FullyManned = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByNameAndLocationShouldReturnOne()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobName = TestJob.JobName,
            JobLocation = TestJob.JobLocation
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByNameAndLocationShouldReturnEmptyForWrongLocation()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobName = TestJob.JobName,
            JobLocation = ""
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByNameAndLocationShouldReturnEmptyForWrongName()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobName = "",
            JobLocation = TestJob.JobLocation
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndLocationShouldReturnOne()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobLocation = TestJob.JobLocation
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndLocationShouldReturnEmptyForWrongLocation()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobLocation = ""
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndLocationShouldReturnEmptyForWrongJobType()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = "",
            JobLocation = TestJob.JobLocation
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndNameShouldReturnOne()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobName = TestJob.JobName
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndNameShouldReturnEmptyForWrongName()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobName = ""
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndNameShouldReturnEmptyForWrongJobType()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = "",
            JobName = TestJob.JobName
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobTypeAndNameAndLocationShouldReturnOne()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobName = TestJob.JobName,
            JobLocation = TestJob.JobLocation
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobStartTimeAndJobTypeShouldReturnOne()
    {
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobStartTime = TestJob.JobStartTime,
            TimeFromStart = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobStartTimeAndJobTypeShouldReturnEmptyForWrongJobType()
    {
        var filter = new JobsFilterParameters()
        {
            JobTypeName = "",
            JobStartTime = TestJob.JobStartTime,
            TimeFromStart = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobStartTimeAndJobTypeShouldReturnEmptyForWrongJobStartTime()
    {
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobStartTime = TestJob.JobStartTime.Value.Subtract(TimeSpan.FromDays(100)),
            TimeFromStart = false,
            TimeBeforeStart = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobStartTimeAndJobTypeShouldReturnEmptyForWrongJobStartTime2()
    {
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobStartTime = DateTime.MaxValue,
            TimeFromStart = true,
            TimeBeforeStart = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobEndTimeAndJobTypeShouldReturnOne()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobEndTime = TestJob.JobEndTime,
            TimeBeforeEnd = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobEndTimeAndJobTypeShouldReturnEmptyForWrongJobType()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = "",
            JobEndTime = TestJob.JobEndTime,
            TimeBeforeEnd = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobEndTimeAndJobTypeShouldReturnEmptyForWrongJobEndTime()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobEndTime = TestJob.JobStartTime.Value.Subtract(TimeSpan.FromDays(100)),
            TimeBeforeEnd = true,
            TimeFromEnd = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobEndTimeAndJobTypeShouldReturnEmptyForWrongJobEndTime2()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobTypeName = TestJob.JobTypeName,
            JobEndTime = DateTime.MaxValue,
            TimeBeforeEnd = false,
            TimeFromEnd = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobMannedStatusAndJobLocationShouldReturnOne()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobLocation = TestJob.JobLocation,
            FullyManned = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobMannedStatusAndJobLocationShouldReturnEmptyForWrongJobLocation()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobLocation = "",
            FullyManned = false
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobsByJobMannedStatusAndJobLocationShouldReturnEmptyForWrongJobMannedStatus()
    {
        // Arrange
        var filter = new JobsFilterParameters()
        {
            JobLocation = TestJob.JobLocation,
            FullyManned = true
        };

        // Act
        var result = _jobsService.GetJobsByFilter(TestCampaign.CampaignGuid.Value, filter).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(5)]
    public void GetJobAssignmentCapableUsersShouldBeEmpty()
    {
        // Arrange

        // Act
        var result = _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, TestJob.JobGuid, false).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(6)]
    public void AddJobAssignmentCapabilityShouldWork()
    {
        // Arrange
        testJobAssignmentCapabilityParams.JobGuid = TestJob.JobGuid.Value;

        // Act
        var result = _jobAssignmentCapabilityService
            .AddJobAssignmentCapableUser(TestCampaign.CampaignGuid.Value, testJobAssignmentCapabilityParams).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(7)]
    public void AddJobAssignmentCapabilityShouldFailForDuplicateKey()
    {
        // Arrange

        // Act
        var result = _jobAssignmentCapabilityService
            .AddJobAssignmentCapableUser(TestCampaign.CampaignGuid.Value, testJobAssignmentCapabilityParams).Result;

        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, result);
    }

    [Fact, TestPriority(7)]
    public void AddJobAssignmentCapabilityShouldFailForWrongJobGuid()
    {
        // Arrange
        var param = new JobAssignmentCapabilityParams()
        {
            JobGuid = Guid.Empty,
            UserEmail = TestUser.Email
        };

        // Act
        var result = _jobAssignmentCapabilityService.AddJobAssignmentCapableUser(TestCampaign.CampaignGuid.Value, param)
            .Result;

        // Assert
        Assert.Equal(CustomStatusCode.JobNotFound, result);
    }

    [Fact, TestPriority(7)]
    public void AddJobAssignmentCapabilityShouldFailForWrongUserEmail()
    {
        // Arrange
        var param = new JobAssignmentCapabilityParams()
        {
            JobGuid = TestJob.JobGuid.Value,
            UserEmail = ""
        };

        // Act
        var result = _jobAssignmentCapabilityService.AddJobAssignmentCapableUser(TestCampaign.CampaignGuid.Value, param)
            .Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }

    [Fact, TestPriority(7)]
    public void GetJobAssignmentCapableUsersShouldReturnOne()
    {
        // Arrange

        // Act
        var result = _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, TestJob.JobGuid, false).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Email == TestUser.Email);
    }

    [Fact, TestPriority(7)]
    public void GetJobAssignmentCapableUsersShouldReturnEmptyForWrongJobGuid()
    {
        // Arrange

        // Act
        var result = _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, Guid.Empty, false).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(7)]
    public void AddJobTypeAssignmentCapableUserShouldWork()
    {
        // Arrange
        TestJobTypeAssignmentParams.JobTypeName = TestJobType.JobTypeName;

        // Act
        var result = _jobTypeAssignmentCapabilityService.AddJobTypeAssignmentCapableUser(
            TestCampaign.CampaignGuid.Value, TestJobTypeAssignmentParams).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(8)]
    public void AddJobTypeAssignmentCapableUserShouldFailForDuplicateKey()
    {
        // Arrange

        // Act
        var result = _jobTypeAssignmentCapabilityService.AddJobTypeAssignmentCapableUser(
            TestCampaign.CampaignGuid.Value, TestJobTypeAssignmentParams).Result;

        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, result);
    }

    [Fact, TestPriority(8)]
    public void AddJobTypeAssignmentCapableUserShouldFailForWrongJobTypeName()
    {
        // Arrange
        var param = new JobTypeAssignmentCapabilityParams()
        {
            JobTypeName = "",
            UserEmail = TestUser.Email
        };

        // Act
        var result = _jobTypeAssignmentCapabilityService.AddJobTypeAssignmentCapableUser(
            TestCampaign.CampaignGuid.Value, param).Result;

        // Assert
        Assert.Equal(CustomStatusCode.JobTypeNotFound, result);
    }

    [Fact, TestPriority(8)]
    public void AddJobTypeAssignmentCapableUserShouldFailForWrongUserEmail()
    {
        // Arrange
        var param = new JobTypeAssignmentCapabilityParams()
        {
            JobTypeName = TestJobType.JobTypeName,
            UserEmail = ""
        };

        // Act
        var result = _jobTypeAssignmentCapabilityService.AddJobTypeAssignmentCapableUser(
            TestCampaign.CampaignGuid.Value, param).Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }

    [Fact, TestPriority(8)]
    public void GetJobTypeAssignmentCapableUsersShouldReturnOne()
    {
        // Arrange

        // Act
        var result = _jobTypeAssignmentCapabilityService.GetJobTypeAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, TestJobType.JobTypeName).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Email == TestUser.Email);
    }

    [Fact, TestPriority(8)]
    public void GetJobTypeAssignmentCapableUsersShouldReturnEmptyForWrongJobTypeName()
    {
        // Arrange

        // Act
        var result = _jobTypeAssignmentCapabilityService.GetJobTypeAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, "").Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(9)]
    public void GetJobAssignmentCapableUsersWithJobTypeShouldReturnTwo()
    {
        // Arrange

        // Act
        var result = _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, TestJob.JobGuid, true).Result;
        _helper.WriteLine(result.ToString());

        var resultList = result.ToList();

        // Assert
        Assert.NotEmpty(resultList);
        Assert.Contains(resultList, x => x.Email == TestUser.Email);
    }

    [Fact, TestPriority(10)]
    public void AddJobAssignmentShouldWork()
    {
        // Arrange

        // Act
        var result = _jobsService.AddJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value,
            TestJobAssignmentParams, TestUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(11)]
    public void AddJobAssignmentShouldFailForDuplicateKey()
    {
        // Arrange

        // Act
        var result = _jobsService.AddJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value,
            TestJobAssignmentParams, TestUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.DuplicateKey, result);
    }

    [Fact, TestPriority(11)]
    public void AddJobAssignmentShouldFailForWrongJobGuid()
    {
        // Arrange

        // Act
        var result = _jobsService.AddJobAssignment(TestCampaign.CampaignGuid.Value, Guid.Empty,
            TestJobAssignmentParams, TestUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.JobNotFound, result);
    }

    [Fact, TestPriority(11)]
    public void AddJobAssignmentShouldFailForWrongUserEmail()
    {
        // Arrange
        var param = new JobAssignmentParams()
        {
            UserEmail = "",
        };

        // Act
        var result = _jobsService.AddJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value,
            param, TestUser.UserId).Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }

    [Fact, TestPriority(11)]
    public void GetJobAssignmentShouldReturnOne()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJobAssignments(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Email == TestUser.Email && x.Salary == TestJob.JobDefaultSalary);
    }

    [Fact, TestPriority(11)]
    public void GetJobAssignmentShouldReturnEmptyForWrongJobGuid()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJobAssignments(TestCampaign.CampaignGuid.Value, Guid.Empty).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(12)]
    public void UpdateJobAssignmentShouldWork()
    {
        // Arrange
        TestJobAssignmentParams.Salary = 1000000;

        // Act
        var result = _jobsService.UpdateJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value,
            TestJobAssignmentParams).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }

    [Fact, TestPriority(13)]
    public void UpdateJobAssignmentShouldFailForWrongJobGuid()
    {
        // Arrange

        // Act
        var result = _jobsService.UpdateJobAssignment(TestCampaign.CampaignGuid.Value, Guid.Empty,
            TestJobAssignmentParams).Result;

        // Assert
        Assert.Equal(CustomStatusCode.JobNotFound, result);
    }

    [Fact, TestPriority(13)]
    public void UpdateJobAssignmentShouldFailForWrongUserEmail()
    {
        // Arrange
        var param = new JobAssignmentParams()
        {
            UserEmail = "",
        };

        // Act
        var result = _jobsService.UpdateJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value,
            param).Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, result);
    }

    [Fact, TestPriority(14)]
    public void GetUpdatedJobAssignmentShouldWork()
    {
        // Arrange

        // Act
        var result = _jobsService.GetJobAssignments(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Email == TestUser.Email && x.Salary == TestJobAssignmentParams.Salary);
    }

    [Fact, TestPriority(15)]
    public void DeleteJobAssignmentShouldFailForWrongJobGuid()
    {
        // Arrange

        // Act
        var res = _jobsService.RemoveJobAssignment(TestCampaign.CampaignGuid.Value, Guid.Empty, TestUser.Email).Result;

        // Assert
        Assert.Equal(CustomStatusCode.JobNotFound, res);
    }

    [Fact, TestPriority(15)]
    public void DeleteJobAssignmentShouldFailForWrongUserEmail()
    {
        // Arrange

        // Act
        var res = _jobsService.RemoveJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value, "").Result;

        // Assert
        Assert.Equal(CustomStatusCode.UserNotFound, res);
    }

    [Fact, TestPriority(15)]
    public void GetUserJobsShouldWork()
    {
        // Arrange

        // Act
        var result = _jobsService.GetUserJobs(TestUser.UserId).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid.Value);
    }

    [Fact, TestPriority(15)]
    public void GetUserJobsShouldReturnEmptyForWrongUserId()
    {
        // Arrange

        // Act
        var result = _jobsService.GetUserJobs(-1).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(15)]
    public void GetUserJobsForSpecificCampaignShouldWork()
    {
        // Arrange

        // Act
        var result = _jobsService.GetUserJobs(TestUser.UserId, TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.JobGuid == TestJob.JobGuid.Value);
    }

    [Fact, TestPriority(15)]
    public void GetUserJobsForSpecificCampaignShouldReturnEmptyForWrongUserId()
    {
        // Arrange

        // Act
        var result = _jobsService.GetUserJobs(-1, TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(15)]
    public void GetUserJobsForSpecificCampaignShouldReturnEmptyForWrongCampaignGuid()
    {
        // Arrange

        // Act
        var result = _jobsService.GetUserJobs(TestUser.UserId, Guid.Empty).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(100)]
    public void DeleteJobAssignmentShouldWork()
    {
        // Arrange

        // Act
        var res = _jobsService
            .RemoveJobAssignment(TestCampaign.CampaignGuid.Value, TestJob.JobGuid.Value, TestUser.Email).Result;

        // Assert
        Assert.Equal(CustomStatusCode.Ok, res);
    }


    [Fact, TestPriority(100)]
    public void DeleteJobTypeAssignmentCapableUserShouldWork2()
    {
        // Arrange

        // Act
        _jobTypeAssignmentCapabilityService
            .RemoveJobTypeAssignmentCapableUser(TestCampaign.CampaignGuid.Value, TestJobTypeAssignmentParams).Wait();
        var result = _jobTypeAssignmentCapabilityService.GetJobTypeAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, TestJobType.JobTypeName).Result;

        // Assert
        Assert.Empty(result);
    }

    [Fact, TestPriority(100)]
    public void DeleteJobAssignmentCapabilityShouldWork2()
    {
        // Arrange

        // Act
        _jobAssignmentCapabilityService
            .RemoveJobAssignmentCapableUser(TestCampaign.CampaignGuid.Value, testJobAssignmentCapabilityParams).Wait();
        var result = _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(
            TestCampaign.CampaignGuid.Value, TestJob.JobGuid, false).Result;

        // Assert
        Assert.Empty(result);
    }


    [Fact, TestPriority(100)]
    public void DeleteJobTypeShouldWork()
    {
        // Arrange

        // Act
        _jobTypesService.DeleteJobType("Test Job Type updated", TestCampaign.CampaignGuid.Value).Wait();
        var result = _jobTypesService.GetJobTypes(TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.DoesNotContain(result, x => x.JobTypeName == "Test Job Type updated");
    }

    [Fact, TestPriority(100)]
    public void DeleteJobShouldWork()
    {
        // Arrange

        // Act
        _jobsService.DeleteJob(TestJob.JobGuid.Value, TestCampaign.CampaignGuid.Value).Wait();
        var job = _jobsService.GetJob(TestJob.JobGuid.Value, TestCampaign.CampaignGuid.Value).Result;

        // Assert
        Assert.Null(job);
    }
}
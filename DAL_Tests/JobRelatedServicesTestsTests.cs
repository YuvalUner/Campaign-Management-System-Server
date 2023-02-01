using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace DAL_Tests;

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
        UserId = 2
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
    
    public JobRelatedServicesTestsTests(ITestOutputHelper helper)
    {
        _configuration = new ConfigurationBuilder().
            SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        _jobAssignmentCapabilityService = new JobAssignmentCapabilityService(new GenericDbAccess(_configuration));
        _jobsService = new JobsService(new GenericDbAccess(_configuration));
        _jobTypesService = new JobTypesService(new GenericDbAccess(_configuration));
        _jobTypeAssignmentCapabilityService = new JobTypeAssignmentCapabilityService(new GenericDbAccess(_configuration));
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
        var result = _jobTypesService.UpdateJobType(TestJobType, TestCampaign.CampaignGuid.Value, "Test Job Type").Result;
        
        // Assert
        Assert.Equal(CustomStatusCode.Ok, result);
    }
    
    [Fact, TestPriority(4)]
    public void UpdateJobTypeShouldFailForDuplicateUniqueIndex()
    {

        // Act
        var result = _jobTypesService.UpdateJobType(TestJobType, TestCampaign.CampaignGuid.Value, "Test Job Type updated").Result;
        
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
        Assert.Contains(result, x => x.JobTypeName == TestJobType.JobTypeName && x.JobTypeDescription == TestJobType.JobTypeDescription);
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

    [Fact, TestPriority(101)]
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
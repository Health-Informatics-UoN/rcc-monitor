using System.Collections;
using Monitor.Data.Entities;
using Monitor.Shared.Models;

namespace Monitor.Tests.Services;

public class StudyCapacityServiceTests : IClassFixture<Fixtures>
{
    private readonly Fixtures _fixtures;

    public StudyCapacityServiceTests(Fixtures fixtures)
    {
        _fixtures = fixtures;
    }
    
    [Fact]
    public async Task IsJobDue_Should_Return_True_When_JobFrequency_Is_Due()
    {
        // Arrange
        var studyCapacityService = _fixtures.GetStudyCapacityService();
        var jobFrequency = TimeSpan.FromHours(24); 
        var lastChecked = DateTimeOffset.Now.AddDays(-2);

        // Act
        var result = await studyCapacityService.IsJobDue(jobFrequency, lastChecked);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsJobDue_Should_Return_False_When_JobFrequency_Is_Not_Due()
    {
        // Arrange
        var studyCapacityService = _fixtures.GetStudyCapacityService();
        var jobFrequency = TimeSpan.FromHours(24); 
        var lastChecked = DateTimeOffset.Now.AddHours(-12); 

        // Act
        var result = await studyCapacityService.IsJobDue(jobFrequency, lastChecked);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task UpdateStudyGroups_Should_Update_Existing_Groups()
    {
        // Arrange
        var studyId = 8;
        var study = new Study
        {
            RedCapId = studyId, 
            ApiKey = "test"
        };
        var existingGroups = new List<StudyGroup>
        {
            new() { Id = 1, Name = "Group 1", PlannedSize = 10, Study = study},
        };

        _fixtures.DbContext.StudyGroups.AddRange(existingGroups);
        await _fixtures.DbContext.SaveChangesAsync();

        var studyCapacityService = _fixtures.GetStudyCapacityService();

        // Act
        var groupsToUpdate = new List<StudyGroupModel>
        {
            new() { Id = 1, Name = "Updated Group 1", PlannedSize = 15 },
        };

        await studyCapacityService.UpdateStudyGroups(studyId, groupsToUpdate);

        // Assert
        var updatedGroups = _fixtures.DbContext.StudyGroups.Single(x => x.Id == 1);
        Assert.Equal("Updated Group 1", updatedGroups.Name);
        Assert.Equal(15, updatedGroups.PlannedSize);
    }
    
    [Fact]
    public async Task CreateStudyGroups_Should_Create_New_Groups()
    {
        // Arrange
        var studyId = 9;
        var study = new Study
        {
            RedCapId = studyId, 
            ApiKey = "test"
        };
        _fixtures.DbContext.Studies.Add(study);
        await _fixtures.DbContext.SaveChangesAsync();
        
        var studyCapacityService = _fixtures.GetStudyCapacityService();

        // Act
        var groupsToAdd = new List<StudyGroupModel>
        {
            new() { Id = 3, Name = "New Group 1", PlannedSize = 5 },
        };

        await studyCapacityService.UpdateStudyGroups(studyId, groupsToAdd);

        // Assert
        var group = _fixtures.DbContext.StudyGroups.Single(x => x.Id == 3);
        Assert.Equal("New Group 1", group.Name);
    }
    
    [Fact]
    public async Task RemoveStudyGroups_Should_Delete_Groups()
    {
        // Arrange
        var studyId = 10;
        var study = new Study
        {
            RedCapId = studyId, 
            ApiKey = "test",
        };
        var existingGroups = new List<StudyGroup>
        {
            new() { Id = 4, Name = "Group 1", PlannedSize = 10, Study = study},
            new() { Id = 5, Name = "Group 2", PlannedSize = 15, Study = study}
        };
        
        _fixtures.DbContext.StudyGroups.AddRange(existingGroups);
        await _fixtures.DbContext.SaveChangesAsync();
        
        var studyCapacityService = _fixtures.GetStudyCapacityService();

        // Act
        var groupsToKeep = new List<StudyGroupModel>
        {
            new() { Id = 4, Name = "Group 1", PlannedSize = 15 },
        };

        await studyCapacityService.UpdateStudyGroups(studyId, groupsToKeep);

        // Assert
        var groups = _fixtures.DbContext.StudyGroups
            .Where(x => x.Study.RedCapId == studyId)
            .ToList();
        
        Assert.Single((IEnumerable)groups);
        Assert.Equal("Group 1", groups[0].Name);
    }
}
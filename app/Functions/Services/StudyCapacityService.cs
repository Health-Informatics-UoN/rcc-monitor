using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Data.Entities;
using Monitor.Shared.Models.Studies;
using Monitor.Shared.Services;

namespace Functions.Services;

public class StudyCapacityService
{
    private readonly ApplicationDbContext _db;
    private readonly RedCapStudyService _redCapStudyService;

    public StudyCapacityService(ApplicationDbContext db, RedCapStudyService redCapStudyService)
    {
        _db = db;
        _redCapStudyService = redCapStudyService;
    }

    /// <summary>
    /// Check if the Study Capacity job is due.
    /// </summary>
    /// <param name="jobFrequency">The frequency that it runs.</param>
    /// <param name="lastChecked">The time it was last checked.</param>
    /// <returns>Whether the job is due to run.</returns>
    public Task<bool> IsJobDue(TimeSpan jobFrequency, DateTimeOffset lastChecked)
    {
        return Task.FromResult((DateTime.Now - jobFrequency) >= lastChecked);
    }

    /// <summary>
    /// Check if a Study is reaching its capacity.  
    /// </summary>
    /// <param name="study">Study to check.</param>
    public async Task CheckStudyCapacity(StudyModel study)
    {
        // Get study groups from RedCap and update the study with them.
        var groups = await _redCapStudyService.GetGroups(study.ApiKey);
        await UpdateStudyGroups(study.Id, groups);
            
        // Get the total study capacity
        var capacity = groups.Sum(x => x.PlannedSize);
            
        // Get the number of subjects from RedCap
        var subjects = await _redCapStudyService.GetSubjectsCount(study.ApiKey);
        
        var entity = await _db.Studies.FindAsync(study.Id);
        if (entity is null) throw new KeyNotFoundException();
        
        // If it has breached the threshold we'll set an alert.
        if (subjects > (capacity * study.StudyCapacityThreshold))
        {
            entity.StudyCapacityAlert = true;
        }
        
        entity.StudyCapacityLastChecked = DateTimeOffset.Now;
        entity.SubjectsEnrolled = subjects;
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Update a Study with its latest Study Groups.
    /// </summary>
    /// <remarks>
    /// Create the group if it does not already exist.
    /// Update the group if it does exist.
    /// Delete the group if it no longer exists. 
    /// </remarks>
    /// <param name="id">Id of the Study to update.</param>
    /// <param name="groups">The groups to update with.</param>
    public async Task UpdateStudyGroups(int id, List<StudyGroupModel> groups)
    {
        var study = _db.Studies
            .Include(x => x.StudyGroups)
            .Single(x => x.RedCapId == id) ?? throw new KeyNotFoundException();

        // Transform existing entities into a dictionary for lookup by Id
        var existingGroupsDictionary = study.StudyGroups.ToDictionary(x => x.Id);

        foreach (var group in groups)
        {
            if (existingGroupsDictionary.TryGetValue(group.Id, out var existingGroup))
            {
                // Update the existing group 
                existingGroup.Name = group.Name;
                existingGroup.PlannedSize = group.PlannedSize;
                
                existingGroupsDictionary.Remove(existingGroup.Id);
            }
            else
            {
                // Create a new group
                var newGroup = new StudyGroup
                {
                    Id = group.Id,
                    Study = study,
                    PlannedSize = group.PlannedSize,
                    Name = group.Name,
                };

                _db.StudyGroups.Add(newGroup);
            }
        }

        // Delete any remaining in the dictionary (no longer in RedCap)
        foreach (var groupToDelete in existingGroupsDictionary.Values)
        {
            _db.StudyGroups.Remove(groupToDelete);
        }

        await _db.SaveChangesAsync();
    }
}
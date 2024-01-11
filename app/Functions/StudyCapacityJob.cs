using Functions.Models;
using Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monitor.Data;
using Monitor.Data.Constants;
using Monitor.Shared.Constants;

namespace Functions;

public class StudyCapacityJob
{
    private readonly ApplicationDbContext _db;
    private readonly StudyCapacityService _studyCapacityService;

    public StudyCapacityJob(ApplicationDbContext db, StudyCapacityService studyCapacityService)
    {
        _db = db;
        _studyCapacityService = studyCapacityService;
    }

    /// <summary>
    /// A worker job to check if Studies are reaching their capacity on RedCap.
    /// </summary>
    /// <remarks>
    /// The function runs every hour, but checks if the job is due per study, based on the requested frequency
    /// and when it was last checked.
    /// Gets the Production Studies with Study Capacity Alerts activated.
    /// Gets the Study Groups for a Study from RedCap, updates them in the database, and sums their capacity.
    /// Checks the Study Audit Logs for enrolled subjects, and counts the unique subject Ids.
    /// If the count > capacity alert threshold - then set the alert on the Study. 
    /// </remarks>
    [Function("StudyCapacityJob")]
    public async Task Run([TimerTrigger("0 0 * * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("StudyCapacityJob");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        
        var studies = await _db.Studies
            .AsNoTracking()
            .Where(x => x.Instance.Name == Instances.Production)
            .Where(x => x.StudyCapacityAlertsActivated == true)
            .ToListAsync();

        var models = studies.Select(x =>
            new StudyModel
            {
                Id = x.RedCapId,
                Name = x.Name,
                ApiKey = x.ApiKey,
                StudyCapacityJobFrequency = x.StudyCapacityJobFrequency,
                StudyCapacityLastChecked = x.StudyCapacityLastChecked,
                StudyCapacityThreshold = x.StudyCapacityThreshold
            }).ToList();

        foreach (var study in models)
        {
            if (await _studyCapacityService.IsJobDue(study.StudyCapacityJobFrequency,
                    study.StudyCapacityLastChecked))
            {
                await _studyCapacityService.CheckStudyCapacity(study);
            }
        }
    }
}
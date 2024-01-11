using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monitor.Data;
using Monitor.Data.Constants;
using Monitor.Shared.Constants;
using Monitor.Shared.Services;

namespace Functions;

public class StudyProductionDataJob(
    ILoggerFactory loggerFactory,
    ApplicationDbContext db,
    RedCapStudyService redCapStudyService)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<StudyProductionDataJob>();

    /// <summary>
    /// A worker job to check if RedCap Build Studies might have production data being entered.
    /// </summary>
    /// <remarks>
    /// The job runs every day at 9am.
    /// Gets all Build Studies
    /// Gets the current admin configured threshold number for Build Studies
    /// Checks the RedCap Study Audit Logs for enrolled subjects, and counts the unique subject Ids.
    /// If the count > alert threshold - then set the alert on the Study. 
    /// </remarks>
    [Function("StudyProductionDataJob")]
    public async Task Run([TimerTrigger("0 0 9 * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        var studies = await db.Studies
            .Where(x => x.Instance.Name == Instances.Build)
            .ToListAsync();

        // Get the threshold from config
        var threshold = await db.Config.Where(x => x.Key == ConfigKey.SubjectsEnrolledInBuildThreshold).SingleAsync();

        foreach (var study in studies)
        {
            // Get the subjects number and set an alert if the threshold is reached.
            var subjects = await redCapStudyService.GetSubjectsCount(study.ApiKey, Instances.Build);
            if (subjects >= int.Parse(threshold.Value))
            {
                study.SubjectsEnrolled = subjects;
                study.ProductionSubjectsEnteredAlert = true;
                await db.SaveChangesAsync();
            }
        }
    }
}
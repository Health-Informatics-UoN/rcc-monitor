using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monitor.Data;
using Monitor.Shared.Models.Studies;
using Monitor.Shared.Services;

namespace Functions;

public class TokenPermissionsValidationJob(ILoggerFactory loggerFactory, ApplicationDbContext db, 
    StudyPermissionsService studyPermissionsService)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<TokenPermissionsValidationJob>();

    /// <summary>
    /// A worker job to check if Studies API tokens have the correct permissions.
    /// </summary>
    /// <remarks>
    /// The function runs every day at 8am, and validates the permissions of every study API token should only have
    /// the permissions that we need.
    /// Currently it does not do anything if the permissions are incorrect.
    /// TODO: Add alerts / notifications once we have them.
    /// </remarks>
    /// <param name="myTimer"></param>
    [Function("TokenPermissionsValidationJob")]
    public async Task Run([TimerTrigger("0 8 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        var studies = await db.Studies.AsNoTracking().ToListAsync();
        
        var models = studies.Select(x =>
            new StudyModel
            {
                Id = x.RedCapId,
                Name = x.Name,
                ApiKey = x.ApiKey
            }).ToList();
        
        foreach (var study in models)
        {
            await studyPermissionsService.ValidatePermissions(study);
        }
    }
}
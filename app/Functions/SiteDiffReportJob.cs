using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Functions.Services;
using Functions.Services.Contracts;
using Monitor.Data.Constants;
using Monitor.Shared.Config;

namespace Functions;

public class SiteDiffReportJob(
    SiteService siteService,
    IDataService redCapSitesService,
    IReportingService reportingService,
    IOptions<RedCapOptions> redCapOptions)
{
    private readonly RedCapOptions _siteOptions = redCapOptions.Value;

    [Function("SiteDiffReportJob")]
    public async Task Run([TimerTrigger("0 * 10 * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("SiteDiffReportJob");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

        // Fetch data
        var uatSites = await redCapSitesService.ListDetail(_siteOptions.BuildUrl, _siteOptions.BuildKey);
        var prodSites = await redCapSitesService.ListDetail(_siteOptions.ProductionUrl, _siteOptions.ProductionKey);
        
        // Sites with different names
        var redCapConflictingNames = siteService.GetConflictingNames(uatSites, prodSites);
        var newNameConflicts = reportingService.ResolveConflicts(redCapConflictingNames, Reports.ConflictingSiteName);
        foreach (var report in newNameConflicts)
        {
            reportingService.Create(report);
        }

        // Sites missing from production
        var conflictingSites = siteService.GetConflictingSites(uatSites, prodSites);
        var newSiteConflicts = reportingService.ResolveConflicts(conflictingSites, Reports.ConflictingSites);
        foreach (var report in newSiteConflicts)
        {
            reportingService.Create(report);
        }

        // Sites with mismatched parents
        var conflictingSiteParents = siteService.GetConflictingParents(uatSites, prodSites);
        var newSiteParentConflicts =
            reportingService.ResolveConflicts(conflictingSiteParents, Reports.ConflictingSiteParent);
        foreach (var report in newSiteParentConflicts)
        {
            reportingService.Create(report);
        }
    }
}

public class MyInfo
{
    public MyScheduleStatus ScheduleStatus { get; set; } = new MyScheduleStatus();

    public bool IsPastDue { get; set; }
}

public class MyScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}
using Data.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Functions.Config;
using Functions.Services;
using Functions.Services.Contracts;

namespace Functions;

public class SiteDiffReportJob
{
    private readonly SiteService _siteService;
    private readonly IDataService _redCapSitesService;
    private readonly IReportingService _reportingService;
    private readonly SiteOptions _siteOptions;
    
    public SiteDiffReportJob(SiteService siteService,
        IDataService redCapSitesService,
        IReportingService reportingService,
        IOptions<SiteOptions> siteOptions
    )
    {
        _siteService = siteService;
        _redCapSitesService = redCapSitesService;
        _reportingService = reportingService;
        _siteOptions = siteOptions.Value;
    }
    [Function("SiteDiffReportJob")]
    public async Task Run([TimerTrigger("0 * 10 * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("SiteDiffReportJob");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

        // Fetch data
        var UATSites = await _redCapSitesService.ListDetail(_siteOptions.UATUrl, _siteOptions.UATKey);
        var prodSites = await _redCapSitesService.ListDetail(_siteOptions.ProductionUrl, _siteOptions.ProductionKey);

        // For testing!
        UATSites[0].Name = "X";
        
        // Sites with different names
        var redCapConflictingNames = _siteService.GetConflictingNames(UATSites, prodSites);
        var newNameConflicts = _reportingService.ResolveConflicts(redCapConflictingNames, Reports.ConflictingSiteName);
        foreach (var report in newNameConflicts)
        {
            _reportingService.Create(report.Item1);
            _reportingService.Create(report.Item2);
        }

        // Sites missing from production
        var conflictingSites = _siteService.GetConflictingSites(UATSites, prodSites);
        var newSiteConflicts = _reportingService.ResolveConflicts(conflictingSites, Reports.ConflictingSites);
        foreach (var report in newSiteConflicts)
        {
            _reportingService.Create(report);
        }

        // Sites with mismatched parents
        var conflictingSiteParents = _siteService.GetConflictingParents(UATSites, prodSites);
        var newSiteParentConflicts =
            _reportingService.ResolveConflicts(conflictingSiteParents, Reports.ConflictingSiteParent);
        foreach (var report in newSiteParentConflicts)
        {
            _reportingService.Create(report.Item1);
            _reportingService.Create(report.Item2);
        }
    }
}

public class MyInfo
{
    public MyScheduleStatus ScheduleStatus { get; set; }

    public bool IsPastDue { get; set; }
}

public class MyScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}
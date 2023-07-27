using System;
using System.Linq;
using System.Threading.Tasks;
using Francois.FunctionApp.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Francois.FunctionApp.Services;
using Francois.FunctionApp.Services.Contracts;
using Microsoft.Extensions.Options;

namespace Francois.FunctionApp;

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
    
    [FunctionName("SiteDiffReportJob")]
    public async Task RunAsync([TimerTrigger("0 * 10 * * *")] TimerInfo myTimer, ILogger log)
    {
        // Fetch real data.
        var UATSites = await _redCapSitesService.ListDetail(_siteOptions.UATUrl, _siteOptions.UATKey); 
        var prodSites = await _redCapSitesService.ListDetail(_siteOptions.ProductionUrl, _siteOptions.ProductionKey);

        // Sites with different names
        var mismatchedNames = _siteService.GetDiffNames(UATSites, prodSites);
        _reportingService.AlertOnMismatchingSiteName(mismatchedNames);

        // Sites missing from production
        var prodSiteIds = prodSites.Select(x => x.SiteId).ToList();
        var missingSites = _siteService.GetMissingIds(UATSites, prodSiteIds);
        _reportingService.AlertOnMismatchingSites(missingSites);
        
        // Sites with mismatched parents
        var mismatchedParents = _siteService.GetDiffParentSiteId(UATSites, prodSites);
        _reportingService.AlertOnMismatchingSiteParent(mismatchedParents);

        Console.WriteLine("Complete.");
    }
}
using System;
using System.Threading.Tasks;
using Francois.FunctionApp.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Francois.FunctionApp.services;
using Francois.FunctionApp.services.Contracts;
using Microsoft.Extensions.Options;

namespace Francois.FunctionApp;

public class SiteDiffReportJob
{
    private readonly SiteService _siteService;
    private readonly IDataService _redCapSitesService;
    private readonly SiteOptions _siteOptions;
    public SiteDiffReportJob(SiteService siteService, IOptions<SiteOptions> siteOptions, IDataService redCapSitesService)
    {
        _siteService = siteService;
        _redCapSitesService = redCapSitesService;
        _siteOptions = siteOptions.Value;
    }
    
    [FunctionName("SiteDiffReportJob")]
    public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
    {
        // Fetch real data.
        var UATSites = await _redCapSitesService.ListDetail(_siteOptions.UATUrl, _siteOptions.UATKey); 
        var prodSites = await _redCapSitesService.ListDetail(_siteOptions.ProductionUrl, _siteOptions.ProductionKey);

        // Sites with different names
        var result = _siteService.GetDiffNames(UATSites, prodSites);

        Console.WriteLine("Complete.");
    }
}
using Functions.Config;
using Functions.Models;
using Functions.Models.Emails;
using Functions.Services.Contracts;
using Microsoft.Extensions.Options;

namespace Functions.Services;

public class EmailService : IReportingService
{
    public void AlertOnMismatchingSites(List<Site> sites)
    {
        throw new System.NotImplementedException();
    }

    public void AlertOnMismatchingSiteParent(List<(Site, Site)> sites)
    {
        throw new System.NotImplementedException();
    }

    public void AlertOnMismatchingSiteName(List<(Site, Site)> sites)
    {
        throw new System.NotImplementedException();
    }

}
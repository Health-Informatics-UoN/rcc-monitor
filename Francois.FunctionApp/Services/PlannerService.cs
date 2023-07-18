using System.Collections.Generic;
using Francois.FunctionApp.Models;
using Francois.FunctionApp.Services.Contracts;

namespace Francois.FunctionApp.Services;

public class PlannerService : IReportingService
{
    public void AlertOnMismatchingSites(List<(Site, Site)> sites)
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
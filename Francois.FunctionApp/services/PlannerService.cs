using System.Collections.Generic;
using Francois.FunctionApp.models;
using Francois.FunctionApp.services.Contracts;

namespace Francois.FunctionApp.services;

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
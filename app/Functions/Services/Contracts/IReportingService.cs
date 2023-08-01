using Functions.Models;

namespace Functions.Services.Contracts;

public interface IReportingService
{
    /// <summary>
    /// Send alert for a list of sites with mismatching site ids.
    /// </summary>
    /// <param name="sites"></param>
    public void AlertOnMismatchingSites(List<Site> sites);
    
    /// <summary>
    /// Send alert for a list of sites with mismatching parent ids.
    /// </summary>
    /// <param name="sites"></param>
    public void AlertOnMismatchingSiteParent(List<(Site, Site)> sites);
    
    /// <summary>
    /// Send alert for a list of sites with mismatching names.
    /// </summary>
    /// <param name="sites"></param>
    public void AlertOnMismatchingSiteName(List<(Site, Site)> sites);
}
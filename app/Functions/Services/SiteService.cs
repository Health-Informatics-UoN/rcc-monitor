using Data.Constants;
using Functions.Models;

namespace Functions.Services;

public class SiteService
{
    /// <summary>
    /// Get a report for missing site Ids from two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns></returns>
    public List<ReportModel> GetConflictingSites(List<Site> sites1, List<Site> sites2)
    {
        var missingSites = sites1
            .Where(s => sites2.All(site => site.SiteId != s.SiteId))
            .Select(site => new ReportModel
            {
                DateTime = DateTimeOffset.UtcNow,
                Description = "",
                ReportTypeModel = Reports.ConflictingSites,
                SiteId = site.SiteId,
                Instance = Instances.Uat,
                ParentSiteId = site.ParentSiteId,
                SiteName = site.Name
            })
            .ToList();
        return missingSites;
    }

    /// <summary>
    /// Get sites with different names but the same Id, given two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of Tuples of Reports</returns>
    public List<(ReportModel, ReportModel)> GetConflictingNames(List<Site> sites1, List<Site> sites2)
    {
        var sitesWithDifferentNames = sites1
            .Join(sites2,
                site1 => site1.SiteId,
                site2 => site2.SiteId,
                (site1, site2) => (site1: new ReportModel
                    {
                        DateTime = DateTimeOffset.UtcNow,
                        Description = "",
                        ReportTypeModel = Reports.ConflictingSiteName,
                        SiteId = site1.SiteId,
                        SiteName = site1.Name,
                        ParentSiteId = site1.ParentSiteId,
                        Instance = Instances.Uat
                    },
                    site2: new ReportModel
                    {
                        DateTime = DateTimeOffset.UtcNow,
                        Description = "",
                        ReportTypeModel = Reports.ConflictingSiteName,
                        SiteId = site2.SiteId,
                        SiteName = site2.Name,
                        ParentSiteId = site1.ParentSiteId,
                        Instance = Instances.Production
                    }))
            .Where(sites => sites.site1.SiteName != sites.site2.SiteName)
            .ToList();

        return sitesWithDifferentNames;
    }

    /// <summary>
    /// Get sites with different parent site Ids from two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of Tuples of Reports</returns>
    public List<(ReportModel, ReportModel)> GetConflictingParents(List<Site> sites1, List<Site> sites2)
    {
        // compare the parent of each site
        var sitesWithDifferentParentSiteId = sites1
            .Join(sites2,
                site1 => site1.SiteId,
                site2 => site2.SiteId,
                (site1, site2) => (report1: new ReportModel
                    {
                        DateTime = DateTimeOffset.UtcNow,
                        ReportTypeModel = Reports.ConflictingSiteParent,
                        SiteId = site1.SiteId,
                        SiteName = site1.Name,
                        ParentSiteId = site1.ParentSiteId,
                        Instance = Instances.Uat
                    },
                    report2: new ReportModel
                    {
                        DateTime = DateTimeOffset.UtcNow,
                        ReportTypeModel = Reports.ConflictingSiteParent,
                        SiteId = site2.SiteId,
                        SiteName = site2.Name,
                        ParentSiteId = site2.ParentSiteId,
                        Instance = Instances.Production
                    }))
            .Where(sites =>
            {
                // Get the parent site for the current site
                var site1Parent = sites1.FirstOrDefault(site => site.SiteId == sites.report1.ParentSiteId.ToString());
                
                // Get prods parent and check if they match
                var site2Parent =
                    sites2.FirstOrDefault(site => site.SiteId == sites.report2.ParentSiteId.ToString());
                
                return site2Parent != null && site1Parent != null &&
                       site2Parent.SiteId != site1Parent.SiteId;
            })
            .ToList();

        return sitesWithDifferentParentSiteId;
    }
}
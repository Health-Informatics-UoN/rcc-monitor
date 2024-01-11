using Functions.Models;
using Monitor.Data.Constants;
using Monitor.Shared.Constants;

namespace Functions.Services;

public class SiteService
{
    /// <summary>
    /// Get a report for missing site Ids from two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of reports.</returns>
    public List<ReportModel> GetConflictingSites(List<Site> sites1, List<Site> sites2)
    {
        var missingSites = sites1
            .Where(s => sites2.All(site => site.SiteId != s.SiteId))
            .Select(site => new ReportModel
            {
                DateTime = DateTimeOffset.UtcNow,
                Description = "",
                ReportTypeModel = Reports.ConflictingSites,
                Sites = new List<SiteModel>
                {
                    new()
                    {
                        SiteId = site.SiteId,
                        Instance = Instances.Uat,
                        ParentSiteId = site.ParentSiteId.ToString(),
                        SiteName = site.Name
                    }
                }
            })
            .ToList();
        return missingSites;
    }

    /// <summary>
    /// Get sites with different names but the same Id, given two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of Reports</returns>
    public List<ReportModel> GetConflictingNames(List<Site> sites1, List<Site> sites2)
    {
        var sitesWithDifferentNames = sites1
            .Join(sites2,
                site1 => site1.SiteId,
                site2 => site2.SiteId,
                (site1, site2) => new ReportModel
                    {
                        DateTime = DateTimeOffset.UtcNow,
                        Description = "",
                        ReportTypeModel = Reports.ConflictingSiteName,
                        Sites = new List<SiteModel>
                        {
                            new()
                            {
                                SiteId = site1.SiteId,
                                Instance = Instances.Uat,
                                ParentSiteId = site1.ParentSiteId.ToString(),
                                SiteName = site1.Name
                            },                        
                            new()
                            {
                                SiteId = site2.SiteId,
                                Instance = Instances.Production,
                                ParentSiteId = site2.ParentSiteId.ToString(),
                                SiteName = site2.Name
                            }
                        }
                    })
            .Where(report => report.Sites[0].SiteName != report.Sites[1].SiteName)
            .ToList();

        return sitesWithDifferentNames;
    }

    /// <summary>
    /// Get sites with different parent site Ids from two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of Tuples of Reports</returns>
    public List<ReportModel> GetConflictingParents(List<Site> sites1, List<Site> sites2)
    {
        // compare the parent of each site
        var sitesWithDifferentParentSiteId = sites1
            .Join(sites2,
                site1 => site1.SiteId,
                site2 => site2.SiteId,
                (site1, site2) => new ReportModel
                {
                    DateTime = DateTimeOffset.UtcNow,
                    ReportTypeModel = Reports.ConflictingSiteParent,
                    Sites = new List<SiteModel>
                    {
                        new()
                        {
                            SiteId = site1.SiteId,
                            SiteName = site1.Name,
                            ParentSiteId = site1.ParentSiteId.ToString(),
                            Instance = Instances.Uat
                        },
                        new()
                        {
                            SiteId = site2.SiteId,
                            SiteName = site2.Name,
                            ParentSiteId = site2.ParentSiteId.ToString(),
                            Instance = Instances.Production
                        }
                    }
                })
            .Where(report =>
            {
                // Get the parent site for the current site
                var site1Parent = sites1.FirstOrDefault(site => site.Id.ToString() == report.Sites[0].ParentSiteId);
                
                // Get prods parent and check if they match
                var site2Parent = sites2.FirstOrDefault(site => site.Id.ToString() == report.Sites[1].ParentSiteId);
                
                return site2Parent != null && site1Parent != null &&
                       site2Parent.SiteId != site1Parent.SiteId;
            })
            .ToList();

        return sitesWithDifferentParentSiteId;
    }
}
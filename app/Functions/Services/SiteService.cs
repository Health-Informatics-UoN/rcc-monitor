using System.Collections.Generic;
using System.Linq;
using Functions.Models;

namespace Functions.Services;

public class SiteService
{
    /// <summary>
    /// Get Sites with missing Ids from two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns></returns>
    public List<Site> GetMissingIds(List<Site> sites1, List<string> sites2)
        => sites1.Where(s => !sites2.Contains(s.SiteId)).ToList();

    /// <summary>
    /// Get sites with different names but the same Id, given two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of Tuples of sites</returns>
    public List<(Site, Site)> GetDiffNames(List<Site> sites1, List<Site> sites2)
    {
        var sitesWithDifferentNames = sites1
            .Join(sites2,
                site1 => site1.SiteId,
                site2 => site2.SiteId,
                (site1, site2) => (site1: site1, site2: site2))
            .Where(sites => sites.site1.Name != sites.site2.Name)
            .ToList();
        
        return sitesWithDifferentNames;
    }

    /// <summary>
    /// Get sites with different parent site Ids from two lists
    /// </summary>
    /// <param name="sites1"></param>
    /// <param name="sites2"></param>
    /// <returns>A list of Tuples of sites</returns>
    public List<(Site, Site)> GetDiffParentSiteId(List<Site> sites1, List<Site> sites2)
    {
        // compare the parent of each site
        var sitesWithDifferentParentSiteId = sites1
            .Where(site1 => site1.ParentSiteId != 0) // Filter out sites without a parent
            .Join(sites2,
                site1 => site1.SiteId,
                site2 => site2.SiteId,
                (site1, site2) => (site1, site2))
            .Where(sites =>
            {
                // Get the parent site for the current site
                var site1Parent = sites1.FirstOrDefault(site => site.SiteId == sites.site1.ParentSiteId.ToString());

                // Get its prod alternate
                var site2 = sites2.FirstOrDefault(site => site.SiteId == sites.site2.SiteId);

                // Get prods parent and check if they match
                var site2Parent =
                    sites2.FirstOrDefault(site => site.SiteId == sites.site2.ParentSiteId.ToString());

                // TODO: this filters out if build or prod dont have parent sites.
                return site2Parent != null && site1Parent != null &&
                       site2Parent.SiteId != site1Parent.SiteId;
            })
            .ToList();

        return sitesWithDifferentParentSiteId;
    }

}
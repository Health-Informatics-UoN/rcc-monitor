using Functions.Models;
using Functions.Services.Contracts;

namespace Functions.Services;

public class PlaceholderDataService : IDataService
{
    public Task<List<SiteView>> List(string url, string token)
    {
        throw new NotImplementedException();
    }

    public Task<Site> Get(string url, string id, string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<Site>> ListDetail(string url, string token)
    {
        var sites = new List<Site>
        {
            // Add sample sites
            new() { Id = 1, SiteId = "R1", Name = "Root" },
            new() { Id = 2, ParentSiteId = 1, SiteId = "site2", Name = "Site 2" },
            new() { Id = 3, ParentSiteId = 2, SiteId = "site3", Name = "Site 3" },
            new() { Id = 4, ParentSiteId = 3, SiteId = "site4", Name = "Site 4" },
            new() { Id = 5, ParentSiteId = 2, SiteId = "site5", Name = "Site 5" },
            new() { Id = 6, ParentSiteId = 2, SiteId = "site6", Name = "Site 6" }
        };

        return Task.FromResult(sites);
    }
}
using Functions.Models;
using Functions.Services.Contracts;

namespace Functions.Services;

public class DummyDataService : IDataService
{
    public Task<List<SiteView>> List(string url, string token)
    {
        throw new NotImplementedException();
    }

    public Task<Site> Get(string url, string id, string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<Site?>> ListDetail(string url, string token)
    {
        var sites = new List<Site>();

        // Add sample sites
        sites.Add(new Site { Id = 1, SiteId = "R1", Name = "Root" });
        sites.Add(new Site { Id = 2, ParentSiteId = 1, SiteId = "site2", Name = "Site 2" });
        sites.Add(new Site { Id = 3, ParentSiteId = 2, SiteId = "site3", Name = "Site 3" });
        sites.Add(new Site { Id = 4, ParentSiteId = 3, SiteId = "site4", Name = "Site 4" });
        sites.Add(new Site { Id = 5, ParentSiteId = 2, SiteId = "site5", Name = "Site 5" });
        sites.Add(new Site { Id = 6, ParentSiteId = 2, SiteId = "site6", Name = "Site 6" });

        return Task.FromResult(sites);
    }
}
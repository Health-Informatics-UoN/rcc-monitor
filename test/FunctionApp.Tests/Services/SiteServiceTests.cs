using Functions.Models;
using Functions.Services;

namespace Francois.Tests.Services;

public class SiteServiceTests
{
    [Fact]
    public void GetMissingIds_ReturnsSitesWithMissingIds()
    {
        // Arrange
        var siteService = new SiteService();
        var sites1 = new List<Site>
        {
            new Site { SiteId = "1" },
            new Site { SiteId = "2" },
            new Site { SiteId = "3" },
        };
        var sites2 = new List<string> { "1", "3" };
        
        // Act
        var result = siteService.GetMissingIds(sites1, sites2);
        
        // Assert
        Assert.Collection(result, site => Assert.Equal("2", site.SiteId));
    }
    
    [Fact]
    public void GetDiffNames_ReturnsSitesWithDifferentNames()
    {
        // Arrange
        var siteService = new SiteService();
        var sites1 = new List<Site>
        {
            new Site { SiteId = "1", Name = "Site A" },
            new Site { SiteId = "2", Name = "Site B" },
            new Site { SiteId = "3", Name = "Site C" }
        };
        var sites2 = new List<Site>
        {
            new Site { SiteId = "1", Name = "Site A" },
            new Site { SiteId = "2", Name = "Site X" },
            new Site { SiteId = "3", Name = "Site C" }
        };

        // Act
        var result = siteService.GetDiffNames(sites1, sites2);

        // Assert
        Assert.Collection(result,
            tuple =>
            {
                Assert.Equal("2", tuple.Item1.SiteId);
                Assert.Equal("Site B", tuple.Item1.Name);
                Assert.Equal("2", tuple.Item2.SiteId);
                Assert.Equal("Site X", tuple.Item2.Name);
            }
        );
    }
    
    [Fact]
    public void GetDiffParentSiteId_ReturnsSitesWithDifferentParentSiteIds()
    {
        // Arrange
        var siteService = new SiteService();
        var sites1 = new List<Site>
        {
            new Site { SiteId = "1", ParentSiteId = 0 },
            new Site { SiteId = "2", ParentSiteId = 1 },
            new Site { SiteId = "3", ParentSiteId = 0 }
        };
        var sites2 = new List<Site>
        {
            new Site { SiteId = "1", ParentSiteId = 0 },
            new Site { SiteId = "2", ParentSiteId = 3 },
            new Site { SiteId = "3", ParentSiteId = 0 }
        };
    
        // Act
        var result = siteService.GetDiffParentSiteId(sites1, sites2);
    
        // Assert
        Assert.Collection(result,
            tuple =>
            {
                Assert.Equal("2", tuple.Item1.SiteId);
                Assert.Equal(1, tuple.Item1.ParentSiteId);
                Assert.Equal("2", tuple.Item2.SiteId);
                Assert.Equal(3, tuple.Item2.ParentSiteId);
            }
        );
    }
}
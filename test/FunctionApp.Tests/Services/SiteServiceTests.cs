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
        var sites2 = new List<Site>
        {
            new Site { SiteId = "1" },
            new Site { SiteId = "3" },
        };

        // Act
        var result = siteService.GetConflictingSites(sites1, sites2);
        
        // Assert
        Assert.Collection(result, report => Assert.Equal("2", report.Sites[0].SiteId));
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
        var result = siteService.GetConflictingNames(sites1, sites2);

        // Assert
        Assert.Collection(result,
            report =>
            {
                Assert.Equal("2", report.Sites[0].SiteId);
                Assert.Equal("Site B", report.Sites[0].SiteName);
                Assert.Equal("2", report.Sites[1].SiteId);
                Assert.Equal("Site X", report.Sites[1].SiteName);
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
            new Site { Id = 1, SiteId = "1", ParentSiteId = 0 },
            new Site { Id = 2, SiteId = "2", ParentSiteId = 1 },
            new Site { Id = 3, SiteId = "3", ParentSiteId = 1 }
        };
        var sites2 = new List<Site>
        {
            new Site { Id = 1, SiteId = "1", ParentSiteId = 0 },
            new Site { Id = 2, SiteId = "2", ParentSiteId = 1 },
            new Site { Id = 3, SiteId = "3", ParentSiteId = 2 }
        };
    
        // Act
        var result = siteService.GetConflictingParents(sites1, sites2);
    
        // Assert
        Assert.Collection(result,
            report =>
            {
                Assert.Equal("3", report.Sites[0].SiteId);
                Assert.Equal("1", report.Sites[0].ParentSiteId);
                Assert.Equal("3", report.Sites[1].SiteId);
                Assert.Equal("2", report.Sites[1].ParentSiteId);
            }
        );
    }
}
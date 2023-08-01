using Functions.Models;
using Functions.Services.Contracts;
using Functions.Services;

namespace Francois.Tests.Services;

public class DummyDataServiceTests
{
    [Fact]
    public async Task ListDetail_ReturnsListOfSites()
    {
        // Arrange
        var url = "sampleUrl";
        var token = "sampleToken";
        IDataService service = new DummyDataService();

        // Act
        var result = await service.ListDetail(url, token);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Site>>(result);
        Assert.Equal(6, result.Count);

        // Assert individual site properties
        Assert.Equal(1, result[0].Id);
        Assert.Equal("R1", result[0].SiteId);
        Assert.Equal("Root", result[0].Name);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("site2", result[1].SiteId);
        Assert.Equal("Site 2", result[1].Name);
        Assert.Equal(1, result[1].ParentSiteId);

        Assert.Equal(3, result[2].Id);
        Assert.Equal("site3", result[2].SiteId);
        Assert.Equal("Site 3", result[2].Name);
        Assert.Equal(2, result[2].ParentSiteId);

        Assert.Equal(4, result[3].Id);
        Assert.Equal("site4", result[3].SiteId);
        Assert.Equal("Site 4", result[3].Name);
        Assert.Equal(3, result[3].ParentSiteId);

        Assert.Equal(5, result[4].Id);
        Assert.Equal("site5", result[4].SiteId);
        Assert.Equal("Site 5", result[4].Name);
        Assert.Equal(2, result[4].ParentSiteId);

        Assert.Equal(6, result[5].Id);
        Assert.Equal("site6", result[5].SiteId);
        Assert.Equal("Site 6", result[5].Name);
        Assert.Equal(2, result[5].ParentSiteId);
    }
}
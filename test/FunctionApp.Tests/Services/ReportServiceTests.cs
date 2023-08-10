using Data.Constants;
using Functions.Models;
using Functions.Services;

namespace Francois.Tests.Services;

public class ReportServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ReportServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestUpdateExistingConflicts_WithResolvedConflictedNames()
    {
        // Arrange
        var reportService = new ReportService(_fixture.DbContext);

        var redCapConflicts = new List<(ReportModel, ReportModel?)>
        {
            (new ReportModel { SiteName = "SiteA", SiteId = "123", Instance = Instances.Uat }, null),
            (new ReportModel { SiteName = "SiteB", SiteId = "123", Instance = Instances.Production }, null),
        };
        
        var mockActiveReports = new List<ReportModel>
        {
            new() { SiteName = "SiteA", SiteId = "123", Instance = Instances.Uat },
            new() { SiteName = "SiteB", SiteId = "123", Instance = Instances.Production }
        };
        
        // Act
        var newConflicts = reportService.UpdateExistingConflicts(redCapConflicts, mockActiveReports);
        
        // Assert
        Assert.Empty(newConflicts);
    }
    
    [Fact]
    public async Task TestUpdateExistingConflicts_WithUnresolvedConflictedNames()
    {
        // Arrange
        var reportService = new ReportService(_fixture.DbContext);
        await _fixture.SeedTestData();
    
        var redCapConflicts = new List<(ReportModel, ReportModel?)>
        {
            (new ReportModel { SiteName = "Site D", SiteId = "456", Instance = Instances.Production }, null),
        };
        
        // Add report models in DB.
        var mockActiveReports = new List<ReportModel>
        {
            new() { SiteName = "SiteA", SiteId = "123", Instance = Instances.Uat, ReportTypeModel = Reports.ConflictingSites, Status = Status.Active },
        };
        foreach (var report in mockActiveReports)
        {
            reportService.Create(report);
        }
    
        var dbActiveReports = reportService.GetActive(Reports.ConflictingSites);
    
        // Act
        var result = reportService.UpdateExistingConflicts(redCapConflicts, dbActiveReports);
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, tuple =>
        {
            Assert.Equal("456", tuple.Item1.SiteId);
            Assert.Equal("Site D", tuple.Item1.SiteName);
        });
    }

    [Fact]
    public async Task TestGetActiveReports()
    {
        // Arrange
        var reportService = new ReportService(_fixture.DbContext);
        await _fixture.SeedTestData();
        
        // Add report models in DB.
        var mockActiveReports = new List<ReportModel>
        {
            new() { SiteName = "SiteA", SiteId = "123", Instance = Instances.Production, ReportTypeModel = Reports.ConflictingSites, Status = Status.Active },
            new() { SiteName = "SiteB", SiteId = "456", Instance = Instances.Production, ReportTypeModel = Reports.ConflictingSiteName, Status = Status.Active },
            new() { SiteName = "SiteC", SiteId = "789", Instance = Instances.Production, ReportTypeModel = Reports.ConflictingSiteParent, Status = Status.Resolved }
        };
        foreach (var report in mockActiveReports)
        {
            reportService.Create(report);
        }
        
        // Act
        var result = reportService.GetActive(Reports.ConflictingSites);
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, model => {Assert.Equal("SiteA",model.SiteName);});
    }
    

}



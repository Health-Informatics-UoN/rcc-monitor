using Functions.Services;
using Monitor.Data.Constants;
using Monitor.Shared.Constants;
using Monitor.Shared.Models.Reports;
using Monitor.Shared.Models.Sites;
using Moq;

namespace Monitor.Tests.Services;

public class ReportServiceTests : IClassFixture<Fixtures>
{
    private readonly Fixtures _fixture;

    public ReportServiceTests(Fixtures fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Test that given pre-existing conflicts, no new duplicated conflicts are returned.
    /// </summary>
    [Fact]
    public void TestUpdateExistingConflicts_WithResolvedConflictedNames()
    {
        // Arrange
        var reportService = new ReportService(_fixture.DbContext, new Mock<IHttpClientFactory>().Object);

        var redCapConflicts = new List<ReportModel>
        {
            new()
            {
                Sites = new List<SiteModel>
                {
                    new() { SiteName = "SiteA", SiteId = "123", Instance = Instances.Uat },
                    new() { SiteName = "SiteB", SiteId = "123", Instance = Instances.Production },
                }
            }
        };

        var mockActiveReports = new List<ReportModel>
        {
            new()
            {
                Sites = new List<SiteModel>
                {
                    new() { SiteName = "SiteA", SiteId = "123", Instance = Instances.Uat },
                    new() { SiteName = "SiteB", SiteId = "123", Instance = Instances.Production },
                }
            }
        };
        
        // Act
        var newConflicts = reportService.UpdateExistingConflicts(redCapConflicts, mockActiveReports);
        
        // Assert
        Assert.Empty(newConflicts);
    }
    
    /// <summary>
    /// Test new conflicts are identified if they do not exist in the DB.
    /// </summary>
    [Fact]
    public async Task TestUpdateExistingConflicts_WithUnresolvedConflictedNames()
    {
        // Arrange
        var reportService = new ReportService(_fixture.DbContext, new Mock<IHttpClientFactory>().Object);
        await _fixture.SeedTestData();
    
        var redCapConflicts = new List<ReportModel>
        {
            new()
            {
                Sites = new List<SiteModel>
                {
                    new() { SiteName = "SiteD", SiteId = "123", Instance = Instances.Uat },
                    new() { SiteName = "SiteE", SiteId = "123", Instance = Instances.Production },
                }
            }
        };
        
        // Add report models in DB.
        var mockActiveReports = new List<ReportModel>
        {
            new()
            {
                ReportType = new ReportTypeModel
                {
                    Name = Reports.ConflictingSiteName
                },
                Status = new ReportStatusModel
                {
                    Name = Status.Active
                },
                Sites = new List<SiteModel>
                {
                    new() { SiteName = "SiteA", SiteId = "456" },
                    new() { SiteName = "SiteB", SiteId = "456" },
                }
            }
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
        Assert.Collection(result, report =>
        {
            Assert.Equal("123", report.Sites[0].SiteId);
            Assert.Equal("SiteD", report.Sites[0].SiteName);
        });
    }
    
    /// <summary>
    /// Test that we retrieve only Reports that are "active".
    /// </summary>
    [Fact]
    public async Task TestGetActiveReports()
    {
        // Arrange
        var reportService = new ReportService(_fixture.DbContext, new Mock<IHttpClientFactory>().Object);
        await _fixture.SeedTestData();
        
        // Add report models in DB.
        var mockActiveReports = new List<ReportModel>
        {
            new() { ReportType = new ReportTypeModel
                {
                    Name = Reports.ConflictingSites
                }, Status = new ReportStatusModel
                {
                    Name = Status.Active
                } 
            },            
            new() { ReportType = new ReportTypeModel
                {
                    Name = Reports.ConflictingSiteName
                }, Status = new ReportStatusModel
                {
                    Name = Status.Active
                } 
            },
            new() { ReportType = new ReportTypeModel
                {
                    Name = Reports.ConflictingSiteParent
                }, Status = new ReportStatusModel
                {
                    Name = Status.Resolved
                } 
            }
        };
        foreach (var report in mockActiveReports)
        {
            reportService.Create(report);
        }
        
        // Act
        var result = reportService.GetActive(Reports.ConflictingSites);
        
        // Assert
        Assert.Single(result);
    }
    

}



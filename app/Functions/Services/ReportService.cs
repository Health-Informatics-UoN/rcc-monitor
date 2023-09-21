using Data.Constants;
using Functions.Models;
using Functions.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Data.Entities;
using Site = Monitor.Data.Entities.Site;

namespace Functions.Services;

public class ReportService : IReportingService
{
    private readonly ApplicationDbContext _db;
    private readonly HttpClient _client;

    public ReportService(
        ApplicationDbContext db, 
        IHttpClientFactory httpClientFactory
        )
    {
        _db = db;
        _client = httpClientFactory.CreateClient("client");
    }
    
    public void Create(ReportModel reportModel)
    {
        var reportType = _db.ReportTypes.First(x => x.Name == reportModel.ReportTypeModel);
        var status = _db.ReportStatus.First(x => x.Name == Status.Active);
        var entity = new Report
        {
            DateTime = reportModel.DateTime,
            Description = reportModel.Description,
            ReportType = reportType,
            Status = status
        };
        
        foreach (var siteModel in reportModel.Sites)
        {
            var instance = _db.Instances.First(x => x.Name == siteModel.Instance);
            var site = new Site
            {
                SiteId = siteModel.SiteId,
                SiteName = siteModel.SiteName,
                Instance = instance,
                ParentSiteId = siteModel.ParentSiteId
            };
            entity.Sites.Add(site);
        }
        
        _db.Reports.Add(entity);
        _db.SaveChanges();
    }

    public List<ReportModel> GetActive(string reportType) =>
        _db.Reports
            .Include(x => x.Sites)
            .Where(x => 
            x.Status.Name == Status.Active && 
            x.ReportType.Name == reportType)
            .Select(x => new ReportModel()
        {
            Id = x.Id,
            DateTime = x.DateTime,
            Description = x.Description,
            ReportTypeModel = x.ReportType.Name,
            Status = x.Status.Name,
            Sites = x.Sites.Select(site => new SiteModel
            {
                Id = site.Id,
                SiteId = site.SiteId,
                SiteName = site.SiteName,
                Instance = site.Instance.Name,
                ParentSiteId = site.ParentSiteId,
            }).ToList()
        }).ToList();
    
    public void UpdateStatus(int reportId, string status)
    {
        var statusEntity = _db.ReportStatus.First(x => x.Name == status);
        var reportEntity = _db.Reports.First(x => x.Id == reportId);
        reportEntity.Status = statusEntity;
        _db.SaveChanges();
    }
    
    public List<ReportModel> ResolveConflicts(List<ReportModel> redCapConflicts, string reportType)
    {
        var existingConflicts = GetActive(reportType);
        var updatedConflicts = UpdateExistingConflicts(redCapConflicts, existingConflicts);
        
        return updatedConflicts;
    }

    public List<ReportModel> UpdateExistingConflicts(List<ReportModel> redCapConflicts, List<ReportModel> existingConflicts)
    {
        foreach (var report in existingConflicts)
        {
            // Check if the report exists in the redCap list based on SiteName, SiteId, and Instance
            var existsInRedCapList = redCapConflicts.Any(redCapReport =>
                redCapReport.Sites.Any(redCapSite =>
                    report.Sites.Any(existingSite =>
                        redCapSite.SiteName == existingSite.SiteName &&
                        redCapSite.SiteId == existingSite.SiteId &&
                        redCapSite.Instance == existingSite.Instance
                    )
                )
            );
        
            // If it exists in the redCap list, it's still active, so remove it from the list.
            if (existsInRedCapList)
            {
                // Remove the conflicting report from the redCap list
                redCapConflicts.RemoveAll(redCapReport =>
                    redCapReport.Sites.Any(redCapSite =>
                        report.Sites.Any(existingSite =>
                            redCapSite.SiteName == existingSite.SiteName &&
                            redCapSite.SiteId == existingSite.SiteId &&
                            redCapSite.Instance == existingSite.Instance
                        )
                    )
                );
            }
            else
            {
                // It's now resolved, so update its status to Status.Resolved
                UpdateStatus(report.Id, Status.Resolved);
            }
        }
        
        return redCapConflicts;
    }

    public async Task SendSummary()
    {
        var response = await _client.GetAsync("Reports/SendSummary");
        
        response.EnsureSuccessStatusCode();
    }
}
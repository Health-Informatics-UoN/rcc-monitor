using Data.Constants;
using Functions.Models;
using Functions.Services.Contracts;
using Monitor.Data;
using Monitor.Data.Entities;

namespace Functions.Services;

public class ReportService : IReportingService
{
    private readonly ApplicationDbContext _db;
    public ReportService(ApplicationDbContext db)
    {
        _db = db;
    }
    public void Create(ReportModel reportModel)
    {
        var reportType = _db.ReportTypes.First(x => x.Name == reportModel.ReportTypeModel);
        var instance = _db.Instances.First(x => x.Name == reportModel.Instance);
        var status = _db.ReportStatus.First(x => x.Name == Status.Active);
        var entity = new Report
        {
            DateTime = reportModel.DateTime,
            Description = reportModel.Description,
            SiteName = reportModel.SiteName,
            SiteId = reportModel.SiteId,
            ReportType = reportType,
            Instance = instance,
            Status = status
        };
        _db.Reports.Add(entity);
        _db.SaveChanges();
    }

    public List<ReportModel> GetActive(string reportType) =>
        _db.Reports.Where(x => 
            x.Status.Name == Status.Active && 
            x.ReportType.Name == reportType)
            .Select(x => new ReportModel()
        {
            Id = x.Id,
            DateTime = x.DateTime,
            SiteName = x.SiteName,
            SiteId = x.SiteId,
            Description = x.Description,
            ReportTypeModel = x.ReportType.Name,
            Instance = x.Instance.Name,
            Status = x.Status.Name
        }).ToList();
    
    public void UpdateStatus(int reportId, string status)
    {
        var statusEntity = _db.ReportStatus.First(x => x.Name == status);
        var reportEntity = _db.Reports.First(x => x.Id == reportId);
        reportEntity.Status = statusEntity;
        _db.SaveChanges();
    }

    public List<(ReportModel, ReportModel)> ResolveConflicts(
        List<(ReportModel, ReportModel?)> redCapConflicts, string reportType)
    {
        var existingConflicts = GetActive(reportType);
        return UpdateExistingConflicts(redCapConflicts, existingConflicts);
    }
    
    public List<ReportModel> ResolveConflicts(List<ReportModel> redCapConflicts, string reportType)
    {
        var existingConflicts = GetActive(reportType);
        
        // Convert to the tuple format and update
        var tupleRedCapConflicts = redCapConflicts.Select(report => (report, (ReportModel)null)).ToList();
        var updatedConflicts = UpdateExistingConflicts(tupleRedCapConflicts, existingConflicts);

        // Convert the result back to non-tuple format
        return updatedConflicts.Select(tuple => tuple.Item1).ToList();
    }

    public List<(ReportModel, ReportModel)> UpdateExistingConflicts(
        List<(ReportModel, ReportModel?)> redCapConflicts, List<ReportModel> existingConflicts)
    {
        foreach (var report in existingConflicts)
        {
            // Check if the report exists in the redCap list based on SiteName, SiteId, and Instance
            var existsInRedCapList = redCapConflicts.Any(redCapReport =>
                (redCapReport.Item1.SiteName == report.SiteName &&
                 redCapReport.Item1.SiteId == report.SiteId &&
                 redCapReport.Item1.Instance == report.Instance) ||
                (redCapReport.Item2?.SiteName == report.SiteName &&
                 redCapReport.Item2.SiteId == report.SiteId &&
                 redCapReport.Item2.Instance == report.Instance));

            // If it exists in the redCap list, it's still active, so remove it from the list.
            if (existsInRedCapList)
            {
                redCapConflicts.RemoveAll(redCapReport =>
                    (redCapReport.Item1.SiteName == report.SiteName &&
                     redCapReport.Item1.SiteId == report.SiteId &&
                     redCapReport.Item1.Instance == report.Instance) ||
                    (redCapReport.Item2?.SiteName == report.SiteName &&
                     redCapReport.Item2.SiteId == report.SiteId &&
                     redCapReport.Item2.Instance == report.Instance));
            }
            else
            {
                // It's now resolved, so update its status to Status.Resolved
                UpdateStatus(report.Id, Status.Resolved);
            }
        }
        
        return redCapConflicts;
    }
}
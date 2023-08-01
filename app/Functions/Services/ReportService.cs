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
    public void AlertOnMismatchingSites(List<ReportModel> reports)
    {
        throw new System.NotImplementedException();
    }

    public void AlertOnMismatchingSiteParent(List<(ReportModel, ReportModel)> reports)
    {
        throw new System.NotImplementedException();
    }

    public void AlertOnMismatchingSiteName(List<(ReportModel, ReportModel)> reports)
    {
        throw new System.NotImplementedException();
    }

    public void Create(ReportModel reportModel)
    {
        var reportType = _db.ReportTypes.First(x => x.Name == reportModel.ReportTypeModel);
        var instance = _db.Instances.First(x => x.Name == reportModel.Instance);
        var entity = new Report
        {
            DateTime = reportModel.DateTime,
            Description = reportModel.Description,
            SiteName = reportModel.SiteName,
            SiteId = reportModel.SiteId,
            ReportType = reportType,
            Instance = instance,
        };
        _db.Reports.Add(entity);
        _db.SaveChanges();
    }
    
}
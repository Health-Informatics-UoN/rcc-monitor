using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Data;
using Monitor.Data.Constants;
using Monitor.Models;
using Monitor.Models.Emails;
using Monitor.Services.Contracts;

namespace Monitor.Services;

public class ReportService
{
    private readonly ApplicationDbContext _db;
    private readonly BaseEmailSenderOptions _siteConfig;
    private readonly IEmailSender _emailSender;

    public ReportService(
        IOptions<BaseEmailSenderOptions> siteConfigOptions, 
        IEmailSender emailSender, ApplicationDbContext db
        )
    {
        _db = db;
        _siteConfig = siteConfigOptions.Value;
        _emailSender = emailSender;
    }

    public async Task<IEnumerable<ReportModel>> List()
    {
        var entity = await _db.Reports
            .Include(x => x.ReportType)
            .Include(x => x.Status)
            .Include(x => x.Sites)
                .ThenInclude(y => y.Instance)
            .OrderByDescending(x => x.DateTime)
            .ToListAsync();

        var result = entity.Select(x => new ReportModel
        {
            Id = x.Id,
            DateTime = x.DateTime,
            LastChecked = x.LastChecked,
            Description = x.Description,
            ReportType = new ReportTypeModel
            {
                Id = x.ReportType.Id,
                Name = x.ReportType.Name
            },
            Status = new ReportStatusModel
            {
                Id = x.Status.Id,
                Name = x.Status.Name
            },
            Sites = x.Sites.Select(y => new SiteModel
            {
                Id = y.Id,
                SiteId = y.SiteId,
                SiteName = y.SiteName,
                Instance = y.Instance.Name,  
                ParentSiteId = y.ParentSiteId ?? string.Empty,
            }).ToList()
        }).ToList();

        return result;
    }

    public async Task<IEnumerable<InstanceModel>> ListInstances()
    {
        var entity = await _db.Instances.ToListAsync();
        return entity.Select(x => new InstanceModel
        {
            Id = x.Id,
            Name = x.Name
        });
    }

    /// <summary>
    /// Sends a summary email of the Redcap reports.
    /// </summary>
    public async Task SendSummary()
    {
        var to = new EmailAddress(_siteConfig.ToAddress);

        var reports = await List();
        var instances = await ListInstances();
        var activeReports  = reports.Where(x => x.Status.Name == Status.Active).ToList();

        // Build the report types.
        var conflictingSites = activeReports.Where(x => x.ReportType.Name == Reports.ConflictingSites).ToList();
        var conflictingNames = activeReports.Where(x => x.ReportType.Name == Reports.ConflictingSiteName).ToList();
        var conflictingParents = activeReports.Where(x => x.ReportType.Name == Reports.ConflictingSiteParent).ToList();

        if (activeReports.Any())
            await _emailSender.SendEmail(to, "Emails/SummaryReport",
                new SummaryReport(conflictingSites, conflictingNames, conflictingParents, instances.ToList()));
    }
    
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Data;
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
            .Include(x => x.Instance)
            .ToListAsync();

        var result = entity.Select(x => new ReportModel
        {
            Id = x.Id,
            DateTime = x.DateTime,
            SiteName = x.SiteName,
            SiteId = x.SiteId,
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
            Instance = new InstanceModel
            {
                Id = x.Instance.Id,
                Name = x.Instance.Name
            }
        });

        return result;
    }

    public async Task ConflictingSiteTriggerReport(List<ReportModel> reports)
    {
        var to = new EmailAddress(_siteConfig.ToAddress);

        await _emailSender.SendEmail(
            to,
            "Emails/SiteIdComparison",
            new AlertOnConflictingSites(reports));
    }

    public async Task ConflictingSiteNameTriggerReport(List<(ReportModel, ReportModel)> reports)
    {
        var to = new EmailAddress(_siteConfig.ToAddress);

        await _emailSender.SendEmail(
            to,
            "Emails/SiteNameComparison",
            new AlertOnConflictingSiteName(reports));
    }
    
    public async Task ConflictingSiteParentTriggerReport(List<(ReportModel, ReportModel)> reports)
    {
        var to = new EmailAddress(_siteConfig.ToAddress);

        await _emailSender.SendEmail(
            to,
            "Emails/SiteParentReport",
            new AlertOnConflictingSiteParent(reports));
    }
}
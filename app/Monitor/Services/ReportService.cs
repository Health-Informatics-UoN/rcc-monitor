using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Models;
using Monitor.Models.Emails;
using Monitor.Services.Contracts;

namespace Monitor.Services;

public class ReportService
{
    private readonly BaseEmailSenderOptions _siteConfig;
    private readonly IEmailSender _emailSender;

    public ReportService(IOptions<BaseEmailSenderOptions> siteConfigOptions, IEmailSender emailSender)
    {
        _siteConfig = siteConfigOptions.Value;
        _emailSender = emailSender;
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
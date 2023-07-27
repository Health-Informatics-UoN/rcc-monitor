using System.Collections.Generic;
using System.Threading.Tasks;
using Francois.FunctionApp.Config;
using Francois.FunctionApp.Models;
using Francois.FunctionApp.Models.Emails;
using Francois.FunctionApp.Services.Contracts;
using Francois.FunctionApp.Services.EmailServices.Contracts;
using Microsoft.Extensions.Options;

namespace Francois.FunctionApp.Services;

public class EmailService : IReportingService
{
    private readonly BaseEmailSenderOptions _siteConfig;
    private readonly IEmailSender _emailSender;
    public EmailService(
        IOptions<BaseEmailSenderOptions> siteConfigOptions, IEmailSender emailSender)
    {
        _siteConfig = siteConfigOptions.Value;
        _emailSender = emailSender;
    }

    public void AlertOnMismatchingSites(List<Site> sites)
    {
        var to = new EmailAddress(_siteConfig.ToAddress);

        Task.WhenAny(_emailSender.SendEmail(
            to,
            "Emails/AlertOnMismatchingSites",
            new AlertOnMismatchingSites(sites)
        ));
    }

    public void AlertOnMismatchingSiteParent(List<(Site, Site)> sites)
    {
        var to = new EmailAddress(_siteConfig.ToAddress);

        Task.WhenAny(_emailSender.SendEmail(
            to,
            "Emails/AlertOnMismatchingSiteParent",
            new AlertOnMismatchingSiteParent(sites)
        ));
    }

    public void AlertOnMismatchingSiteName(List<(Site, Site)> sites)
    {
        var to = new EmailAddress(_siteConfig.ToAddress);
        
        Task.WhenAny(_emailSender.SendEmail(
            to,
            "Emails/AlertOnMismatchingSiteName",
            new AlertOnMismatchingSiteName(sites)
        ));
    }

}
using System.Threading.Tasks;
using Francois.FunctionApp.Config;
using Francois.FunctionApp.Models.Emails;
using Francois.FunctionApp.Models.Emails.ReferenceData;
using Francois.FunctionApp.Services.EmailServices.Contracts;
using Microsoft.Extensions.Options;

namespace Francois.FunctionApp.Services.EmailServices;

public class EmailService : IEmailService
{
    private readonly SitePropertiesOptions _siteConfig;
    private readonly IEmailSender _emailSender;

    public EmailService(
        IOptions<SitePropertiesOptions> siteConfigOptions, IEmailSender emailSender)
    {
        _siteConfig = siteConfigOptions.Value;
        _emailSender = emailSender;
    }

    public async Task SendAlertOnMismatchingSites(EmailAddress to, string name, string entity)
    {
        await _emailSender.SendEmail(
            to,
            "Emails/AlertOnMismatchingSites",
            new AlertOnMismatchingSites
            {
                Name = name,
                Entity = entity
            }
        );
    }

    public async Task SendAlertOnMismatchingSiteParent(EmailAddress to, string name, string entity)
    {
        await _emailSender.SendEmail(
            to,
            "Emails/AlertOnMismatchingSiteParent",
            new AlertOnMismatchingSiteParent
            {
                Name = name,
                Entity = entity
            }
        );
    }

    public async Task SendAlertOnMismatchingSiteName(EmailAddress to, string name, string entity)
    {
        await _emailSender.SendEmail(
            to,
            "Emails/AlertOnMismatchingSiteName",
            new AlertOnMismatchingSiteName
            {
                Name = name,
                Entity = entity
            }
        );
    }

}
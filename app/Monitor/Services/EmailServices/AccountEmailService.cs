using Monitor.Models.Emails;
using Monitor.Services.Contracts;

namespace Monitor.Services.EmailServices;

public class AccountEmailService
{
  private readonly IEmailSender _emails;

  public AccountEmailService(IEmailSender emails)
  {
    _emails = emails;
  }

  public async Task SendAccountConfirmation(EmailAddress to, string link, string resendLink)
      => await _emails.SendEmail(
          to,
          "Emails/AccountConfirmation",
          new TokenEmailModel(
            to.Name!,
            link,
            resendLink));

  public async Task SendPasswordReset(EmailAddress to, string link, string resendLink)
      => await _emails.SendEmail(
          to,
          "Emails/PasswordReset",
          new TokenEmailModel(
            to.Name!,
            link,
            resendLink));

  public async Task SendEmailChange(EmailAddress to, string link)
      => await _emails.SendEmail(
          to,
          //$"Confirm your new {_config.ServiceName} Email Address",
          "Emails/EmailChangeConfirmation",
          new TokenEmailModel(
            to.Name!,
            link,
            string.Empty));
  
  public async Task SendUserInvite(EmailAddress to, string link)
    => await _emails.SendEmail(
      to,
      //$"Confirm your new {_config.ServiceName} Email Address",
      "Emails/InviteUser",
      new TokenEmailModel(
        to.Name!,
        link,
        string.Empty));
  
  public async Task SendDeleteUpdate(EmailAddress to)
    => await _emails.SendEmail(
      to,
      //$"Confirm your new {_config.ServiceName} Email Address",
      "Emails/DeleteUpdate",
      new TokenEmailModel(
        to.Name!,
        string.Empty,
        string.Empty));
}


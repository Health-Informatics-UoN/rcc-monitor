using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Monitor.Config;
using Monitor.Services;
using Monitor.Services.Contracts;
using Monitor.Services.EmailSender;
using Monitor.Services.EmailServices;

namespace Monitor.Extensions
{
  public static class ServiceCollectionExtensions
  {

    public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
    {

      var emailProvider = c["OutboundEmail:Provider"] ?? string.Empty;

      var useSendGrid = emailProvider.Equals("sendgrid", StringComparison.InvariantCultureIgnoreCase);

      if (useSendGrid) s.Configure<SendGridOptions>(c.GetSection("OutboundEmail"));
      else s.Configure<LocalDiskEmailOptions>(c.GetSection("OutboundEmail"));

      s
              // .AddTransient<TokenIssuingService>()
              .AddTransient<RazorViewService>()
              .AddTransient<AccountEmailService>()
              .TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      if (useSendGrid) s.AddTransient<IEmailSender, SendGridEmailSender>();
      else s.AddTransient<IEmailSender, LocalDiskEmailSender>();

      return s;
    }
  }
}

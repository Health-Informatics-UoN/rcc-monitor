using Monitor.Models.Emails;

namespace Monitor.Services.Contracts
{
  public interface IEmailSender
  {
    /// <summary>
    /// Send an email (compiled from a Razor View with a Model)
    /// to a single email address,
    /// using the default From account.
    /// </summary>
    /// <typeparam name="TModel">The Type of the ViewModel the View expects</typeparam>
    /// <param name="toAddress">The email address to send to</param>
    /// <param name="viewName">a Razor View to compile to form the email content</param>
    /// <param name="model">a ViewModel instance for the specified View</param>
    Task SendEmail<TModel>(
        EmailAddress toAddress,
        string viewName,
        TModel model)
        where TModel : class;

    /// <summary>
    /// Send an email (compiled from a Razor View with a Model)
    /// to multiple addresses,
    /// using the default From account.
    /// </summary>
    /// <typeparam name="TModel">The Type of the ViewModel the View expects</typeparam>
    /// <param name="toAddresses">The email addresses to send to</param>
    /// <param name="viewName">a Razor View to compile to form the email content</param>
    /// <param name="model">a ViewModel instance for the specified View</param>
    Task SendEmail<TModel>(
        List<EmailAddress> toAddresses,
        string viewName,
        TModel model)
        where TModel : class;
  }
}

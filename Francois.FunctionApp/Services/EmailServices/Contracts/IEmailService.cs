using System.Threading.Tasks;
using Francois.FunctionApp.Models.Emails;
using Francois.FunctionApp.Services.Contracts;

namespace Francois.FunctionApp.Services.EmailServices.Contracts
{
    public interface IEmailService
    {
        Task SendAlertOnMismatchingSites(EmailAddress to, string name, string entity);
        Task SendAlertOnMismatchingSiteParent(EmailAddress to, string name, string entity);
        Task SendAlertOnMismatchingSiteName(EmailAddress to, string name, string entity);
    };
}

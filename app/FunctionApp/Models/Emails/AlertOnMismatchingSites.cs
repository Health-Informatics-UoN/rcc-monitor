using System.Collections.Generic;

namespace Francois.FunctionApp.Models.Emails;

public record AlertOnMismatchingSites(List<Site> Sites);
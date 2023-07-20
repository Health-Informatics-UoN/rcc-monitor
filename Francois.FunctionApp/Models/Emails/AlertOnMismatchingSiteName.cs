using System.Collections.Generic;

namespace Francois.FunctionApp.Models.Emails;

public record AlertOnMismatchingSiteName(List<(Site, Site)> Sites);
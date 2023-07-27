using System.Collections.Generic;

namespace Francois.FunctionApp.Models.Emails;

public record AlertOnMismatchingSiteParent(List<(Site, Site)> Sites);
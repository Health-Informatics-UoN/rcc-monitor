using System.Collections.Generic;

namespace Functions.Models.Emails;

public record AlertOnMismatchingSiteName(List<(Site, Site)> Sites);

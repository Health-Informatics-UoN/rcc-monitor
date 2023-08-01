using System.Collections.Generic;

namespace Functions.Models.Emails;

public record AlertOnMismatchingSiteParent(List<(Site, Site)> Sites);

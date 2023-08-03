namespace Monitor.Models.Emails;

public record AlertOnConflictingSiteParent(List<(ReportModel, ReportModel)> Reports);

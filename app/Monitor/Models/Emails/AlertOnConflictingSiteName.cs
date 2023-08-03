namespace Monitor.Models.Emails;

public record AlertOnConflictingSiteName(List<(ReportModel, ReportModel)> Reports);

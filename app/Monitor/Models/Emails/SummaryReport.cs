namespace Monitor.Models.Emails;

public record SummaryReport(List<ReportModel> ConflictingSites, List<ReportModel> ConflictingNames,
    List<ReportModel> ConflictingParents, List<InstanceModel> Instances);

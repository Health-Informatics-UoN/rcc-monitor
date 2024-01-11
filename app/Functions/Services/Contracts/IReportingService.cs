using Monitor.Shared.Models.Reports;

namespace Functions.Services.Contracts;

public interface IReportingService
{
    /// <summary>
    /// Save report to the database
    /// </summary>
    /// <param name="reportModel">Report to save</param>
    public void Create(ReportModel reportModel);

    /// <summary>
    /// Get a list of "Active" reports for a given report type.
    /// </summary>
    /// <returns>A list of reports that are "Active".</returns>
    public List<ReportModel> GetActive(string reportType);
    
    /// <summary>
    /// Update the status for a report.
    /// </summary>
    /// <param name="reportId">ID of the report to update</param>
    /// <param name="status">The new status of the report.</param>
    public void UpdateStatus(int reportId, string status);

    /// <summary>
    /// Resolves conflicts by:
    /// update conflicts that already exist in the database,
    /// return conflicts that do not exist.
    /// </summary>
    /// <param name="redCapConflicts">A list of conflicts.</param>
    /// <param name="reportType">The type of report to update.</param>
    /// <returns>A list of reports that do not exist in the database, so are new conflicts.</returns>
    public List<ReportModel> ResolveConflicts(List<ReportModel> redCapConflicts, string reportType);

    /// <summary>
    /// Resolves conflicts by:
    /// update conflicts that already exist in the database,
    /// return conflicts that do not exist.
    /// </summary>
    /// <param name="redCapConflicts">List of reports to check.</param>
    /// <param name="existingConflicts">List of existing reports in the database.</param>
    /// <returns>A list of reports that do not exist in the database, so are new conflicts.</returns>
    public List<ReportModel> UpdateExistingConflicts(List<ReportModel> redCapConflicts, List<ReportModel> existingConflicts);

    /// <summary>
    /// Requests the Webapp controller to send a summary email.
    /// </summary>
    public Task SendSummary();
}
using Functions.Models;

namespace Functions.Services.Contracts;

public interface IReportingService
{
    /// <summary>
    /// Send alert for a list of sites with mismatching site ids.
    /// </summary>
    /// <param name="reports"></param>
    public void AlertOnMismatchingSites(List<ReportModel> reports);
    
    /// <summary>
    /// Send alert for a list of sites with mismatching parent ids.
    /// </summary>
    /// <param name="reports"></param>
    public void AlertOnMismatchingSiteParent(List<(ReportModel, ReportModel)> reports);
    
    /// <summary>
    /// Send alert for a list of sites with mismatching names.
    /// </summary>
    /// <param name="reports"></param>
    public void AlertOnMismatchingSiteName(List<(ReportModel, ReportModel)> reports);

    /// <summary>
    /// Save report to the database
    /// </summary>
    /// <param name="reportModel">Report to save</param>
    public void Create(ReportModel reportModel);
    
}
namespace Monitor.Models;

public class ReportModel
{
    public int Id { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public ReportTypeModel ReportType { get; set; } = new();
    public ReportStatusModel Status { get; set; } = new();
    public List<SiteModel> Sites { get; set; } = new List<SiteModel>();
}

namespace Monitor.Models;

public class ReportModel
{
    public int Id { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportTypeModel ReportType { get; set; } = new();
    public InstanceModel Instance { get; set; } = new();
}
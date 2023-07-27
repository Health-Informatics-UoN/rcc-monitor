namespace Monitor.Data.Entities;

public class Report
{
    public int Id { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType ReportType { get; set; } = new();
}

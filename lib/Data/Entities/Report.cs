namespace Monitor.Data.Entities;

public class Report
{
    public int Id { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public DateTimeOffset LastChecked { get; set; }
    public string Description { get; set; } = string.Empty;
    public ReportType ReportType { get; set; } = new();
    public ReportStatus Status { get; set; } = new();
    public ICollection<Site> Sites { get; set; } = new List<Site>();
}

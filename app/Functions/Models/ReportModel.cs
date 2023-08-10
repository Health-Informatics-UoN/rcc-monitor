namespace Functions.Models;

public class ReportModel
{
    public int Id { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public int ParentSiteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReportTypeModel { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

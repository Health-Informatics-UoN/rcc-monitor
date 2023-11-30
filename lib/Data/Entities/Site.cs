namespace Monitor.Data.Entities;

public class Site
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public string? ParentSiteId { get; set; } = string.Empty;
    public Instance Instance { get; set; } = new();
}

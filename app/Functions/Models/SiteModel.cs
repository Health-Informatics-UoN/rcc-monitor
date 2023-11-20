namespace Functions.Models;

public class SiteModel
{
    public int Id { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public string ParentSiteId { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
}

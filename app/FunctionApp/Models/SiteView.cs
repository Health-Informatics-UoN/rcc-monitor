namespace Francois.FunctionApp.Models;

/// <summary>
/// A partial model of a site, mirrored from redCAP.
/// </summary>
public class SiteView
{
    public int Id { get; set; }
    public string? SiteName { get; set; }
    public string? Facility { get; set; }
    public string? SiteType { get; set; }
    public bool Enabled { get; set; }
}

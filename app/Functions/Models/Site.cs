
namespace Functions.Models;

/// <summary>
/// A complete model of a site, mirrored from redCAP.
/// </summary>
public class Site
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int ParentSiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public int TimeZoneId { get; set; }
    public int SiteTypeId { get; set; }
    public bool Enabled { get; set; }
    public bool Opened { get; set; }
    public string? Locale { get; set; }
    public string? Summary { get; set; }
    public string? FacilityName { get; set; }
    public string? FacilityCity { get; set; }
    public string? FacilityState { get; set; }
    public string? FacilityZip { get; set; }
    public string? FacilityCountry { get; set; }
    public string? FacilityContactName { get; set; }
    public string? FacilityContactDegree { get; set; }
    public string? FacilityPhone { get; set; }
    public string? FacilityEmail { get; set; }
    public List<object>? Children { get; set; }
    public string? SiteTypeLabel { get; set; }
    public string? ParentSiteName { get; set; }
    public string? TimeZoneName { get; set; }
    public int ParentId { get; set; }
    public string? Caption { get; set; }
}

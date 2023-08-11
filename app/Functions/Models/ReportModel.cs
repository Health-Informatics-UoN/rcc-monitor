namespace Functions.Models;

public class ReportModel
{
    public int Id { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReportTypeModel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<SiteModel> Sites { get; set; } = new List<SiteModel>();
}

using System.Text.Json.Serialization;

namespace Monitor.Shared.Models;

public class AuditLogsFilter
{
    [JsonPropertyName("auditEventTypes")]
    public List<int> AuditEventTypes { get; set; } = new List<int>();
    
    [JsonPropertyName("auditEventTypeNames")]
    public List<string> AuditEventTypeNames { get; set; } = new List<string>();
    
    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;
    
    [JsonPropertyName("endDate")]
    public string EndDate { get; set; } = string.Empty;
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }
}

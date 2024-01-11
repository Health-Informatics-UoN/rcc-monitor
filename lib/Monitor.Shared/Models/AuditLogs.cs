namespace Monitor.Shared.Models;

public class AuditLogs
{
    public int TotalItems { get; set; }
    public List<AuditSubject> Records { get; set; } = new List<AuditSubject>();
}

public record AuditSubject
{
    public int SubjectId { get; set; }
}

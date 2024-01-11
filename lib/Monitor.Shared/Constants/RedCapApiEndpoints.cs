namespace Monitor.Shared.Constants;

/// <summary>
/// RedCap API Endpoints
/// </summary>
public static class RedCapApiEndpoints
{
    private const string ApiBase = "/rest/v2";
    public const string Studies = $"{ApiBase}/studies";
    public const string StudyGroups = $"{ApiBase}/study-group-clases";
    public const string AuditLogsEventTypes = $"{ApiBase}/audit-logs/event-types";
    public const string AuditLogs = $"{ApiBase}/audit-logs/filter";
}
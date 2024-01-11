namespace Monitor.Shared.Constants;

/// <summary>
/// RedCap API Endpoints
/// </summary>
internal static class RedCapApiEndpoints
{
    private const string ApiBase = "/rest/v2";
    internal const string StudyGroups = $"{ApiBase}/study-group-clases";
    internal const string AuditLogsEventTypes = $"{ApiBase}/audit-logs/event-types";
    internal const string AuditLogs = $"{ApiBase}/audit-logs/filter";
}
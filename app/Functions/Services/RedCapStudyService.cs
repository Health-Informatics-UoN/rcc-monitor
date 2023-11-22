using Flurl.Http;
using Functions.Config;
using Functions.Constants;
using Functions.Models;
using Microsoft.Extensions.Options;

namespace Functions.Services;

public class RedCapStudyService
{
    private readonly SiteOptions _siteOptions;

    public RedCapStudyService(IOptions<SiteOptions> siteOptions)
    {
        _siteOptions = siteOptions.Value;
    }

    /// <summary>
    /// Get a list of study groups for a given study.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <returns>List of StudyGroups for the Study.</returns>
    public async Task<List<StudyGroupModel>> GetGroups(string token)
    {
        var url = _siteOptions.ProductionUrl + RedCapApiEndpoints.StudyGroups;
        return await url.WithHeader("token", token).GetJsonAsync<List<StudyGroupModel>>();
    }
    
    /// <summary>
    /// Get the number of subjects enrolled in a study.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <returns>Number of subjects in the given study.</returns>
    public async Task<int> GetSubjectsCount(string token)
    {
        var filter = await BuildAuditLogsFilter(token, AuditEventTypeNames.SubjectCreated);
        var subjects = await GetAllAuditLogs(token, filter);

        // Get the count of unique subject Ids 
        var count = subjects.Select(auditSubject => auditSubject.SubjectId).Distinct().ToList().Count;

        return count;
    }
    
    /// <summary>
    /// Builds an <see cref="AuditLogsFilter"/> for a given event Type.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="eventType"></param>
    /// <returns>A filter for audit logs.</returns>
    public async Task<AuditLogsFilter> BuildAuditLogsFilter(string token, string eventType)
    {
        var auditEventId = await GetAuditEventType(token, eventType);
        return new AuditLogsFilter
        {
            AuditEventTypes = new List<int>
            {
                auditEventId
            },
            AuditEventTypeNames = new List<string>
            {
                eventType
            },
            StartDate = "2017-01-01",
            EndDate = DateTime.Now.ToString("yyyy-MM-dd"),
            PageSize = 50,
            PageNumber = 1
        };
    }
    
    /// <summary>
    /// Get Audit Event Type Id from RedCap for a given Event Type.
    /// </summary>
    /// <remarks>
    /// We must be sure we filter logs with the correct Id, so we get it to be safe.
    /// </remarks>
    /// <param name="token">Study access token. Used to get current study ID parameter</param>
    /// <param name="name">The event type name to get the Id of.</param>
    /// <returns>The Id of the given event type name.</returns>
    public async Task<int> GetAuditEventType(string token, string name)
    {
        var url = _siteOptions.ProductionUrl + RedCapApiEndpoints.AuditLogsEventTypes;
        var eventTypes = await url.WithHeader("token", token).GetJsonAsync<List<AuditLogEventType>>();
        return eventTypes.First(x => x.Name == name).Id;
    }

    /// <summary>
    /// Get all the audit logs with a given filter.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="initialFilter">Initial Audit Logs filter to use.</param>
    /// <returns>List of audit subjects returned from RedCap.</returns>
    public async Task<List<AuditSubject>> GetAllAuditLogs(string token, AuditLogsFilter initialFilter)
    {
        var url = _siteOptions.ProductionUrl + RedCapApiEndpoints.AuditLogs;

        var allRecords = new List<AuditSubject>();
        var pageNumber = 1;

        while (true)
        {
            initialFilter.PageNumber = pageNumber;

            var auditLogs = await url.WithHeader("token", token).PostJsonAsync(initialFilter).ReceiveJson<AuditLogs>();

            if (auditLogs.Records.Count == 0)
            {
                // No more logs
                break;
            }

            allRecords.AddRange(auditLogs.Records);
            pageNumber++;
        }

        return allRecords;
    }
}
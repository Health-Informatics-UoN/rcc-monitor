using Flurl.Http;
using Functions.Config;
using Functions.Constants;
using Functions.Models;
using Microsoft.Extensions.Options;
using Monitor.Data.Constants;

namespace Functions.Services;

public class RedCapStudyService(IOptions<SiteOptions> siteOptions)
{
    private readonly SiteOptions _siteOptions = siteOptions.Value;

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
    /// <param name="instance">RedCap instance to check</param>
    /// <returns>Number of subjects in the given study.</returns>
    public async Task<int> GetSubjectsCount(string token, string instance = Instances.Production)
    {
        var auditEventId = await GetAuditEventType(token, AuditEventTypeNames.SubjectCreated, instance);
        var filter = await BuildAuditLogsFilter(auditEventId, AuditEventTypeNames.SubjectCreated);
        var subjects = await GetAllAuditLogs(token, filter, instance);

        // Get the count of unique subject Ids 
        var count = subjects.Select(auditSubject => auditSubject.SubjectId).Distinct().ToList().Count;

        return count;
    }

    /// <summary>
    /// Builds an <see cref="AuditLogsFilter"/> for a given event Type.
    /// </summary>
    /// <param name="auditEventId"></param>
    /// <param name="eventType"></param>
    /// <returns>A filter for audit logs.</returns>
    public Task<AuditLogsFilter> BuildAuditLogsFilter(int auditEventId, string eventType)
    {
        return Task.FromResult(new AuditLogsFilter
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
        });
    }

    /// <summary>
    /// Get Audit Event Type Id from RedCap for a given Event Type.
    /// </summary>
    /// <remarks>
    /// We must be sure we filter logs with the correct Id, so we get it to be safe.
    /// </remarks>
    /// <param name="token">Study access token. Used to get current study ID parameter</param>
    /// <param name="name">The event type name to get the Id of.</param>
    /// <param name="instance">RedCap instance to access.</param>
    /// <returns>The Id of the given event type name.</returns>
    public async Task<int> GetAuditEventType(string token, string name, string instance)
    {
        var url = instance == Instances.Production ? _siteOptions.ProductionUrl : _siteOptions.UatUrl + RedCapApiEndpoints.AuditLogsEventTypes;
        var eventTypes = await url.WithHeader("token", token).GetJsonAsync<List<AuditLogEventType>>();
        return eventTypes.First(x => x.Name == name).Id;
    }

    /// <summary>
    /// Get all the audit logs with a given filter.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="initialFilter">Initial Audit Logs filter to use.</param>
    /// <param name="instance">RedCap instance to access.</param>
    /// <returns>List of audit subjects returned from RedCap.</returns>
    public async Task<List<AuditSubject>> GetAllAuditLogs(string token, AuditLogsFilter initialFilter,
        string instance)
    {
        var url = instance == Instances.Production ? _siteOptions.ProductionUrl : _siteOptions.UatUrl + RedCapApiEndpoints.AuditLogs;

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
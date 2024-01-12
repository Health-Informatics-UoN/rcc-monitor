using Flurl.Http;
using Microsoft.Extensions.Options;
using Monitor.Shared.Config;
using Monitor.Shared.Constants;
using Monitor.Shared.Models;
using Monitor.Shared.Models.AuditLog;
using Monitor.Shared.Models.Studies;

namespace Monitor.Shared.Services;

public class RedCapStudyService(IOptions<RedCapOptions> redCapOptions)
{
    private readonly RedCapOptions _siteOptions = redCapOptions.Value;

    /// <summary>
    /// Get Api Url for an instance of RedCap.
    /// </summary>
    /// <param name="instance">Instance of RedCap to get.</param>
    /// <returns>RedCap Api Url</returns>
    private string GetApiUrl(string instance)
    {
        return instance == Instances.Production
            ? _siteOptions.ProductionUrl
            : _siteOptions.BuildUrl;
    }
    
    /// <summary>
    /// Get Tenant Token for an instance of RedCap.
    /// </summary>
    /// <param name="instance">Instance of RedCap to get.</param>
    /// <returns>Tenant token for the instance.</returns>
    private string GetTenantToken(string instance)
    {
        return instance == Instances.Production
            ? _siteOptions.ProductionKey
            : _siteOptions.BuildKey;
    }

    /// <summary>
    /// Get a Study for a given token on a RedCap instance.
    /// </summary>
    /// <param name="instance">RedCap instance to get from.</param>
    /// <param name="token">Study token to use.</param>
    /// <returns>The Study that matches the token from RedCap</returns>
    public async Task<StudyModel> GetStudy(string instance, string token)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.Studies;
        return await url.WithHeader("token", token).GetJsonAsync<StudyModel>();
    }

    /// <summary>
    /// Get a list of study groups for a given study.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="instance">RedCap instance to access.</param>
    /// <returns>List of StudyGroups for the Study.</returns>
    public async Task<List<StudyGroupModel>> GetGroups(string token, string instance = Instances.Production)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.StudyGroups;
        return await url.WithHeader("token", token).GetJsonAsync<List<StudyGroupModel>>();
    }

    /// <summary>
    /// Get the number of subjects enrolled in a study.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="instance">RedCap instance to access./</param>
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
    private Task<AuditLogsFilter> BuildAuditLogsFilter(int auditEventId, string eventType)
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
    private async Task<int> GetAuditEventType(string token, string name, string instance)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.AuditLogsEventTypes;
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
    private async Task<List<AuditSubject>> GetAllAuditLogs(string token, AuditLogsFilter initialFilter,
        string instance)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.AuditLogs;

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

    /// <summary>
    /// Get Study Assignments for a Study from RedCap.
    /// </summary>
    /// <remarks>
    /// This is the only way to access a User Id from a Study Token if you don't already know it.
    /// This is why the tenant key is necessary to get it.
    /// </remarks>
    /// <param name="study">Study to get the assignments for.</param>
    /// <returns>A list of Study Assignments.</returns>
    public async Task<List<StudyAssignment>> GetStudyAssignments(StudyModel study)
    {
        var url = GetApiUrl(study.Instance) + RedCapApiEndpoints.StudyAssignments(study.Id);
        return await url.WithHeader("token", GetTenantToken(study.Instance)).GetJsonAsync<List<StudyAssignment>>();
    }

    /// <summary>
    /// Get User Assignments (roles) for a Study from RedCap.
    /// </summary>
    /// <param name="study">Study to get assignments for.</param>
    /// <param name="userId">User Id to get assignments for.</param>
    /// <returns>A list of User Assignments for the Study.</returns>
    public async Task<List<StudyAssignment>> GetStudyUserAssignments(StudyModel study, int userId)
    {
        var url = GetApiUrl(study.Instance) + RedCapApiEndpoints.StudyUserAssignments(study.Id, userId);
        return await url.WithHeader("token", GetTenantToken(study.Instance)).GetJsonAsync<List<StudyAssignment>>();
    }

    /// <summary>
    /// Get Study Role details from RedCap.
    /// </summary>
    /// <param name="study">Study to get the role for.</param>
    /// <param name="roleId">Role Id to get the role.</param>
    /// <returns>The Study role for the Id.</returns>
    public async Task<StudyRole> GetStudyRole(StudyModel study, int roleId)
    {
        var url = GetApiUrl(study.Instance) + RedCapApiEndpoints.StudyRoles(study.Id, roleId);
        return await url.WithHeader("token", GetTenantToken(study.Instance)).GetJsonAsync<StudyRole>();
    }
}
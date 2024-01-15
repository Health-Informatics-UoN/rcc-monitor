using Flurl.Http;
using Microsoft.Extensions.Options;
using Monitor.Shared.Config;
using Monitor.Shared.Constants;
using Monitor.Shared.Models;
using Monitor.Shared.Models.AuditLog;
using Monitor.Shared.Models.Studies;

namespace Monitor.Shared.Services;

public class RedCapStudyService(IOptions<RedCapOptions> redCapOptions) : IRedCapStudyService
{
    private readonly RedCapOptions _siteOptions = redCapOptions.Value;
    
    public string GetApiUrl(string instance)
    {
        return instance == Instances.Production
            ? _siteOptions.ProductionUrl
            : _siteOptions.BuildUrl;
    }
    
    public string GetTenantToken(string instance)
    {
        return instance == Instances.Production
            ? _siteOptions.ProductionKey
            : _siteOptions.BuildKey;
    }
    
    public async Task<StudyModel> GetStudy(string instance, string token)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.Studies;
        return await url.WithHeader("token", token).GetJsonAsync<StudyModel>();
    }
    
    public async Task<List<StudyGroupModel>> GetGroups(string token, string instance = Instances.Production)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.StudyGroups;
        return await url.WithHeader("token", token).GetJsonAsync<List<StudyGroupModel>>();
    }
    
    public async Task<int> GetSubjectsCount(string token, string instance = Instances.Production)
    {
        var auditEventId = await GetAuditEventType(token, AuditEventTypeNames.SubjectCreated, instance);
        var filter = await BuildAuditLogsFilter(auditEventId, AuditEventTypeNames.SubjectCreated);
        var subjects = await GetAllAuditLogs(token, filter, instance);

        // Get the count of unique subject Ids 
        var count = subjects.Select(auditSubject => auditSubject.SubjectId).Distinct().ToList().Count;

        return count;
    }
    
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
    
    public async Task<int> GetAuditEventType(string token, string name, string instance)
    {
        var url = GetApiUrl(instance) + RedCapApiEndpoints.AuditLogsEventTypes;
        var eventTypes = await url.WithHeader("token", token).GetJsonAsync<List<AuditLogEventType>>();
        return eventTypes.First(x => x.Name == name).Id;
    }
    
    public async Task<List<AuditSubject>> GetAllAuditLogs(string token, AuditLogsFilter initialFilter,
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
    
    public async Task<List<StudyAssignment>> GetStudyAssignments(StudyModel study)
    {
        var url = GetApiUrl(study.Instance) + RedCapApiEndpoints.StudyAssignments(study.Id);
        return await url.WithHeader("token", GetTenantToken(study.Instance)).GetJsonAsync<List<StudyAssignment>>();
    }
    
    public async Task<List<StudyAssignment>> GetStudyUserAssignments(StudyModel study, int userId)
    {
        var url = GetApiUrl(study.Instance) + RedCapApiEndpoints.StudyUserAssignments(study.Id, userId);
        return await url.WithHeader("token", GetTenantToken(study.Instance)).GetJsonAsync<List<StudyAssignment>>();
    }
    
    public async Task<StudyRole> GetStudyRole(StudyModel study, int roleId)
    {
        var url = GetApiUrl(study.Instance) + RedCapApiEndpoints.StudyRoles(study.Id, roleId);
        return await url.WithHeader("token", GetTenantToken(study.Instance)).GetJsonAsync<StudyRole>();
    }
}
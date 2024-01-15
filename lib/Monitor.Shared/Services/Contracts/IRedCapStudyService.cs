using Monitor.Shared.Constants;
using Monitor.Shared.Models.AuditLog;
using Monitor.Shared.Models.Studies;

namespace Monitor.Shared.Services;

public interface IRedCapStudyService
{
    /// <summary>
    /// Get Api Url for an instance of RedCap.
    /// </summary>
    /// <param name="instance">Instance of RedCap to get.</param>
    /// <returns>RedCap Api Url</returns>
    string GetApiUrl(string instance);

    /// <summary>
    /// Get Tenant Token for an instance of RedCap.
    /// </summary>
    /// <param name="instance">Instance of RedCap to get.</param>
    /// <returns>Tenant token for the instance.</returns>
    string GetTenantToken(string instance);

    /// <summary>
    /// Get a Study for a given token on a RedCap instance.
    /// </summary>
    /// <param name="instance">RedCap instance to get from.</param>
    /// <param name="token">Study token to use.</param>
    /// <returns>The Study that matches the token from RedCap</returns>
    Task<StudyModel> GetStudy(string instance, string token);

    /// <summary>
    /// Get a list of study groups for a given study.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="instance">RedCap instance to access.</param>
    /// <returns>List of StudyGroups for the Study.</returns>
    Task<List<StudyGroupModel>> GetGroups(string token, string instance = Instances.Production);

    /// <summary>
    /// Get the number of subjects enrolled in a study.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="instance">RedCap instance to access./</param>
    /// <returns>Number of subjects in the given study.</returns>
    Task<int> GetSubjectsCount(string token, string instance = Instances.Production);

    /// <summary>
    /// Builds an <see cref="AuditLogsFilter"/> for a given event Type.
    /// </summary>
    /// <param name="auditEventId"></param>
    /// <param name="eventType"></param>
    /// <returns>A filter for audit logs.</returns>
    Task<AuditLogsFilter> BuildAuditLogsFilter(int auditEventId, string eventType);

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
    Task<int> GetAuditEventType(string token, string name, string instance);

    /// <summary>
    /// Get all the audit logs with a given filter.
    /// </summary>
    /// <param name="token">Study access token. Used to get current study ID parameter.</param>
    /// <param name="initialFilter">Initial Audit Logs filter to use.</param>
    /// <param name="instance">RedCap instance to access.</param>
    /// <returns>List of audit subjects returned from RedCap.</returns>
    Task<List<AuditSubject>> GetAllAuditLogs(string token, AuditLogsFilter initialFilter,
        string instance);

    /// <summary>
    /// Get Study Assignments for a Study from RedCap.
    /// </summary>
    /// <remarks>
    /// This is the only way to access a User Id from a Study Token if you don't already know it.
    /// This is why the tenant key is necessary to get it.
    /// </remarks>
    /// <param name="study">Study to get the assignments for.</param>
    /// <returns>A list of Study Assignments.</returns>
    Task<List<StudyAssignment>> GetStudyAssignments(StudyModel study);

    /// <summary>
    /// Get User Assignments (roles) for a Study from RedCap.
    /// </summary>
    /// <param name="study">Study to get assignments for.</param>
    /// <param name="userId">User Id to get assignments for.</param>
    /// <returns>A list of User Assignments for the Study.</returns>
    Task<List<StudyAssignment>> GetStudyUserAssignments(StudyModel study, int userId);

    /// <summary>
    /// Get Study Role details from RedCap.
    /// </summary>
    /// <param name="study">Study to get the role for.</param>
    /// <param name="roleId">Role Id to get the role.</param>
    /// <returns>The Study role for the Id.</returns>
    Task<StudyRole> GetStudyRole(StudyModel study, int roleId);
}
using Microsoft.Extensions.Options;
using Monitor.Shared.Config;
using Monitor.Shared.Constants;
using Monitor.Shared.Exceptions;
using Monitor.Shared.Models.Studies;

namespace Monitor.Shared.Services;

public class StudyPermissionService(
    IOptions<RedCapOptions> config,
    IRedCapStudyService redCapStudyService)
{
    private readonly RedCapOptions _config = config.Value;
    
    /// <summary>
    /// Validate the Permissions for a Study.
    /// Check that the Study API Key only has permission to:
    /// Study Groups: Read
    /// Study Audit Logs: Read
    /// Assign Users: Read, Update.
    /// </summary>
    /// <param name="study">The Study to validate permissions for.</param>
    /// <exception cref="MissingPermissionsException">The study has missing permissions.</exception>
    /// <exception cref="ExtraPermissionsException">The study has additional permissions.</exception>
    public async Task ValidatePermissions(StudyModel study)
    {
        // This is the only way to get the user Id from the API.
        var studyAssignments = await redCapStudyService.GetStudyAssignments(study);
        var userId = studyAssignments.First(x => x.email == _config.ApiEmail).userId;

        var studyUserAssignments = await redCapStudyService.GetStudyUserAssignments(study, userId);

        // Define the permissions we currently accept, add more if needed.
        var requiredPermissions = new Dictionary<string, List<string>>
        {
            { ComponentName.AuditLogs, [AllowedPermissions.ResourcePermissionRead] },
            { ComponentName.StudyGroups, [AllowedPermissions.ResourcePermissionRead] },
            { ComponentName.AssignUsers, [
                AllowedPermissions.ResourcePermissionRead,
                AllowedPermissions.ResourcePermissionUpdate
            ]}
        };

        var (permissionsResult, extraPermissions) =
            await UnwrapPermissions(studyUserAssignments, requiredPermissions, study);

        if (!permissionsResult.Values.All(result => result))
        {
            throw new MissingPermissionsException("The API user does not have the correct permissions.");
        }

        if (extraPermissions.Count > 0)
        {
            throw new ExtraPermissionsException(
                $"The API User has additional permissions: {string.Join(", ", extraPermissions)}.");
        }
    }

    /// <summary>
    /// Unwraps the RedCap Assignment object to check the correct permissions exist.
    /// </summary>
    /// <param name="studyUserAssignments">The Study Assignments from RedCap.</param>
    /// <param name="requiredPermissions">The permissions we want to validate.</param>
    /// <param name="study">Study to unwrap permissions for.</param>
    /// <returns>A tuple of the permissions given, and the extra permissions.</returns>
    public async Task<(Dictionary<string, bool> permissionsResult, List<string> extraPermissions)> UnwrapPermissions(
        IEnumerable<StudyAssignment> studyUserAssignments,
        IReadOnlyDictionary<string, List<string>> requiredPermissions,
        StudyModel study)
    {
        // Keep track of permissions, and add them as we unwrap them.
        var permissionsResult = new Dictionary<string, bool>();
        var extraPermissions = new List<string>();

        foreach (var assignment in studyUserAssignments)
        {
            var role = await redCapStudyService.GetStudyRole(study, assignment.roleId);

            foreach (var permission in role.permissions)
            {
                if (requiredPermissions.TryGetValue(permission.ComponentName, out var allowedPermissions))
                {
                    permissionsResult[permission.ComponentName] = CheckPermissions(permission, allowedPermissions);
                }
                else
                {
                    extraPermissions.Add(permission.ComponentName);
                }
            }
        }

        return (permissionsResult, extraPermissions);
    }


    /// <summary>
    /// Check if the Study Role Component has only the required permissions.
    /// </summary>
    /// <param name="permission">The existing permissions on the Study Role Component.</param>
    /// <param name="allowedPermissions">A list of permissions we require: <see cref="AllowedPermissions"/></param>
    /// <returns>True if the required permissions only exist on the Study Role Component.</returns>
    public bool CheckPermissions(StudyRoleComponentPermissions permission, List<string> allowedPermissions)
    {
        var matchingPermissions = permission.AllowedPermissions
            .Where(x => allowedPermissions.Contains(x.Name))
            .ToList();

        // Check if the count of matching permissions is equal to the count of allowed permissions
        var isCountMatch = matchingPermissions.Count == allowedPermissions.Count;

        // Check if all matching permissions are assigned to the role
        var areAllAssigned =
            matchingPermissions.All(matchingPermission => permission.Permissions.Contains(matchingPermission.Id));

        // Check if all assigned permissions are part of the matching permissions
        var areAllPartOfMatching = permission.Permissions.All(assignedPermissionId =>
            matchingPermissions.Any(matchingPermission => matchingPermission.Id == assignedPermissionId));

        return isCountMatch && areAllAssigned && areAllPartOfMatching;
    }
}
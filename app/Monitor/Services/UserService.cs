using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Constants;
using Monitor.Data;
using Monitor.Models.Users;
using Monitor.Shared.Models.Studies;


namespace Monitor.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    private readonly IKeycloakUserClient _keycloakUserClient;
    private readonly KeycloakOptions _keycloakOptions;

    public UserService(ApplicationDbContext db,
        IKeycloakUserClient keycloakUserClient,
        IOptions<KeycloakOptions> keycloakOptions)
    {
        _db = db;
        _keycloakUserClient = keycloakUserClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    /// <summary>
    /// Get a list of Users from Keycloak.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <returns>List of users, filtered according to query parameters.</returns>
    private async Task<IEnumerable<User>> GetUsers(GetUsersRequestParameters? query)
        => await _keycloakUserClient.GetUsers(_keycloakOptions.Realm, query);

    /// <summary>
    /// Get a list of StudyUsers given StudyUsers entities.
    /// </summary>
    /// <param name="userIds">List of study users entities.</param>
    /// <returns>A list of study users.</returns>
    public async Task<List<StudyUser>> GetStudyUsers(IEnumerable<Data.Entities.StudyUser> userIds)
    {
        var users = new List<StudyUser>();
        foreach (var user in userIds)
        {
            var keycloakUser = await _keycloakUserClient.GetUser(_keycloakOptions.Realm, user.UserId);
            if (keycloakUser.Id != null)
            {
                users.Add(new StudyUser
                {
                    Id = keycloakUser.Id,
                    Name = $"{keycloakUser.FirstName} {keycloakUser.LastName}",
                    Email = keycloakUser.Email ?? string.Empty
                });
            }
        }

        return users;
    }

    /// <summary>
    /// Get a list of users who:
    /// <list type="bullet">
    /// <item><description>Email matches the query.</description></item>
    /// <item><description>Are a trial manager.</description></item>
    /// <item><description>Are not an admin user.</description></item>
    /// <item><description>Are not affiliated with a study.</description></item>
    /// </list>
    /// </summary>
    /// <param name="model">The model to filter by.</param>
    /// <returns>List of users who satisfy the criteria.</returns>
    /// <exception cref="KeyNotFoundException">Study not found.</exception>
    public async Task<List<StudyUser>> GetUnaffiliatedUsers(UnaffiliatedQuery model)
    {
        var users = await GetUsers(new GetUsersRequestParameters
        {
            Email = model.Query
        });
        
        // Filter users, include only trial managers and non-admins.
        var filteredUsers = new List<User>();
        foreach (var user in users)
        {
            if (user.Id == null) continue;
            var userGroups = await _keycloakUserClient.GetUserGroups(_keycloakOptions.Realm, user.Id);
            var groups = userGroups as Group[] ?? userGroups.ToArray();
            
            if (groups.Any(x => x.Name == Groups.TrialManager) && groups.All(x => x.Name != Groups.Admin))
            {
                filteredUsers.Add(user);
            }
        }
        
        // get the matching study
        var study = await _db.Studies
            .AsNoTracking()
            .Include(study => study.Users)
            .Where(x => x.RedCapId == model.StudyId)
            .FirstAsync() ?? throw new KeyNotFoundException();

        // filter users who are not attached to the study
        return filteredUsers
            .ExceptBy(study.Users.Select(su => su.UserId), u => u.Id)
            .Select(y => new StudyUser
            {
                Id = y.Id!,
                Name = $"{y.FirstName} {y.LastName}",
                Email = y.Email ?? string.Empty
            }).ToList();
        
    }
}
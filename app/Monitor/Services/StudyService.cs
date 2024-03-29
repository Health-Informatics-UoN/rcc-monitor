using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Flurl.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Monitor.Auth;
using Monitor.Shared.Constants;
using Monitor.Data.Entities;
using Monitor.Shared.Config;
using Monitor.Shared.Exceptions;
using Monitor.Shared.Models.Studies;
using Monitor.Shared.Services;
using StudyUser = Monitor.Data.Entities.StudyUser;

namespace Monitor.Services;

public class StudyService(
    ApplicationDbContext db,
    IOptions<RedCapOptions> config,
    UserService userService,
    ConfigService configService,
    IAuthorizationService authorizationService,
    IRedCapStudyService redCapStudyService)
{
    private readonly RedCapOptions _config = config.Value;

    /// <summary>
    /// Get a study with a given id.
    /// </summary>
    /// <param name="id">Id of the study to get.</param>
    /// <param name="userId">User to filter by.</param>
    /// <returns>Study with the given id.</returns>
    public async Task<StudyPartialModel> Get(int id, string? userId = null)
    {
        var entity = await db.Studies
                         .AsNoTracking()
                         .Include(x => x.Users)
                         .Include(x => x.Instance)
                         .Include(x => x.StudyGroups)
                         .Where(x => x.RedCapId == id)
                         .Where(x => userId == null || x.Users.Any(s => s.UserId == userId))
                         .SingleOrDefaultAsync()
                     ?? throw new KeyNotFoundException();

        // get users from keycloak
        var users = await userService.GetStudyUsers(entity.Users);

        var model = new StudyPartialModel
        {
            Id = entity.RedCapId,
            Name = entity.Name,
            Users = users,
            StudyGroup = entity.StudyGroups.Select(x => new StudyGroupModel
            {
                Id = x.Id,
                Name = x.Name,
                PlannedSize = x.PlannedSize
            }).ToList(),
            Instance = entity.Instance.Name,
            SubjectsEnrolled = entity.SubjectsEnrolled,
            StudyCapacityAlert = entity.StudyCapacityAlert,
            ProductionSubjectsEnteredAlert = entity.ProductionSubjectsEnteredAlert,
            StudyCapacityAlertsActivated = entity.StudyCapacityAlertsActivated,
            StudyCapacityThreshold = entity.StudyCapacityThreshold,
            SubjectsEnrolledThreshold = entity.SubjectsEnrolledThreshold,
            StudyCapacityJobFrequency = entity.StudyCapacityJobFrequency.ToString(@"hh\:mm\:ss"),
            StudyCapacityLastChecked = entity.StudyCapacityLastChecked.ToString("u")
        };
        return model;
    }

    /// <summary>
    /// Get the list of studies for a relevant user.
    /// </summary>
    /// <param name="userId">User to filter by.</param>
    /// <returns>List of studies.</returns>
    public async Task<IEnumerable<StudyPartialModel>> List(string? userId = null)
    {
        var list = await db.Studies
            .Include(x => x.Users)
            .Include(x => x.Instance)
            .Where(x => userId == null || x.Users.Any(s => s.UserId == userId))
            .ToListAsync();

        var result = list.Select(x => new StudyPartialModel
        {
            Id = x.RedCapId,
            Name = x.Name,
            Instance = x.Instance.Name,
            StudyCapacityAlert = x.StudyCapacityAlert,
            ProductionSubjectsEnteredAlert = x.ProductionSubjectsEnteredAlert
        });

        return result;
    }

    /// <summary>
    /// Delete a study by RedCapId.
    /// </summary>
    /// <param name="redCapId">The RedCapId of the study to delete.</param>
    /// <returns></returns>
    public async Task DeleteStudy(int redCapId)
    {
        var study = await db.Studies.FindAsync(redCapId)
                    ?? throw new KeyNotFoundException($"No study with the RedCap ID: \"{redCapId}\" was found.");

        db.Studies.Remove(study);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new Study from the given model and user Id.
    /// </summary>
    /// <remarks>
    /// If the study already exists with the given Id, we add the user to it.
    /// </remarks>
    /// <param name="model">Study to create.</param>
    /// <param name="userId">User ID to add to the study.</param>
    public async Task<StudyModel> Create(StudyModel model, string userId)
    {
        // If the study already exists just add the user to it.
        var check = await db.Studies.FindAsync(model.Id);
        if (check is not null)
        {
            await AddUser(check.RedCapId, userId);
        }
        else
        {
            var instance = db.Instances.Single(x => x.Name == model.Instance) ?? throw new KeyNotFoundException();

            // Get the current config defaults
            var defaultCapacityThreshold = await configService.GetValue(ConfigKey.RandomisationThreshold, "0.70");
            var defaultCapacityFrequency = await configService.GetValue(ConfigKey.RandomisationJobFrequency, "23:00");
            var defaultSubjectsEnrolledThreshold =
                await configService.GetValue(ConfigKey.SubjectsEnrolledThreshold, "10");

            var entity = new Study
            {
                ApiKey = model.ApiKey,
                Name = model.Name,
                RedCapId = model.Id,
                Instance = instance,
                StudyCapacityThreshold = double.Parse(defaultCapacityThreshold),
                StudyCapacityJobFrequency = TimeSpan.Parse(defaultCapacityFrequency),
                SubjectsEnrolledThreshold = int.Parse(defaultSubjectsEnrolledThreshold)
            };
            db.Studies.Add(entity);

            var user = new StudyUser
            {
                UserId = userId,
                Study = entity,
            };
            db.StudyUsers.Add(user);

            await db.SaveChangesAsync();
        }

        return model;
    }

    /// <summary>
    /// Add users to a study.
    /// </summary>
    /// <param name="id">Id of the study to update.</param>
    /// <param name="model">New model to update with.</param>
    /// <returns>The updated study model.</returns>
    /// <exception cref="KeyNotFoundException">Study not found.</exception>
    public async Task<StudyPartialModel> AddUsers(int id, StudyPartialModel model)
    {
        var entity = await db.Studies
                         .Where(s => s.RedCapId == id)
                         .Include(study => study.Users)
                         .FirstOrDefaultAsync() ??
                     throw new KeyNotFoundException();

        if (model.Users != null)
        {
            var usersToAdd = model.Users
                .Where(newUser => entity.Users.All(existingUser => existingUser.UserId != newUser.Id));

            foreach (var user in usersToAdd)
            {
                await AddUser(id, user.Id);
            }
        }

        await db.SaveChangesAsync();

        return model;
    }

    /// <summary>
    /// Remove users from a study.
    /// </summary>
    /// <param name="id">Id of the study to update.</param>
    /// <param name="model">New model to update with.</param>
    /// <returns>The updated study model.</returns>
    /// <exception cref="KeyNotFoundException">Study not found.</exception>
    public async Task<StudyPartialModel> RemoveUsers(int id, StudyPartialModel model)
    {
        var entity = await db.Studies
                         .Where(s => s.RedCapId == id)
                         .Include(study => study.Users)
                         .FirstOrDefaultAsync() ??
                     throw new KeyNotFoundException();

        var usersToRemove = entity.Users
            .Where(existingUser => model.Users != null && model.Users.All(newUser => newUser.Id != existingUser.UserId))
            .ToList();

        foreach (var user in usersToRemove)
        {
            entity.Users.Remove(user);
        }

        await db.SaveChangesAsync();

        return model;
    }

    /// <summary>
    /// Update the study capacity configuration for a study.
    /// </summary>
    /// <param name="id">Id of the study to update.</param>
    /// <param name="model">New model to update with.</param>
    /// <exception cref="KeyNotFoundException">Study not found</exception>
    /// <exception cref="ArgumentException">Frequency format invalid.</exception>
    public async Task UpdateStudyCapacityConfig(int id, StudyPartialModel model)
    {
        var entity = await db.Studies
                         .Where(s => s.RedCapId == id)
                         .Include(study => study.Users)
                         .FirstOrDefaultAsync() ??
                     throw new KeyNotFoundException();

        entity.StudyCapacityAlertsActivated = model.StudyCapacityAlertsActivated;

        if (model.StudyCapacityAlertsActivated)
        {
            entity.StudyCapacityThreshold = model.StudyCapacityThreshold;
            if (TimeSpan.TryParse(model.StudyCapacityJobFrequency, out var jobFrequency))
            {
                entity.StudyCapacityJobFrequency = jobFrequency;
            }
            else
            {
                throw new ArgumentException("Invalid frequency format");
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Update a study
    /// </summary>
    /// <param name="id">Id of the study to update.</param>
    /// <param name="model">New model to update with.</param>
    /// <param name="user">The user</param>
    /// <returns>The updated study partial model.</returns>
    public async Task<StudyPartialModel> Update(int id, StudyPartialModel model, ClaimsPrincipal user)
    {
        await UpdateStudyCapacityConfig(id, model);

        var response = await AddUsers(id, model);

        // Remove users only if they have permission.
        var authorizationResult =
            await authorizationService.AuthorizeAsync(user, nameof(AuthPolicies.CanRemoveStudyUsers));

        if (authorizationResult.Succeeded)
        {
            response = await RemoveUsers(id, model);
        }

        return response;
    }


    /// <summary>
    /// Add a User to an existing Study.
    /// </summary>
    /// <param name="studyId">Study ID to add to.</param>
    /// <param name="userId">UserId to add to it.</param>
    public async Task AddUser(int studyId, string userId)
    {
        var entity = await db.Studies
            .Where(x => x.RedCapId == studyId)
            .FirstAsync();

        var user = new StudyUser
        {
            UserId = userId,
            Study = entity,
        };

        db.StudyUsers.Add(user);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Validate the Study's APIKey against RedCap.
    /// We try to validate against both environments.
    /// </summary>
    /// <param name="apiKey">APIKey to validate.</param>
    /// <returns>The study attached to the APIKey.</returns>
    /// <exception cref="UnauthorizedAccessException">The APIKey is not authorized with RedCap.</exception>
    /// <exception cref="Exception">We failed to reach RedCap.</exception>
    public async Task<StudyModel> Validate(string apiKey)
    {
        try
        {
            return await ValidateWithInstance(apiKey, Instances.Production);
        }
        catch (FlurlHttpException productionEx) when (productionEx.Call.Response.StatusCode == 401)
        {
            try
            {
                return await ValidateWithInstance(apiKey, Instances.Build);
            }
            catch (FlurlHttpException buildEx) when (buildEx.Call.Response.StatusCode == 401)
            {
                throw new UnauthorizedAccessException("The API Key is not authorized with RedCap on both environments.",
                    buildEx);
            }
        }
    }

    /// <summary>
    /// Validate the Study's APIKey against RedCap.
    /// </summary>
    /// <param name="apiKey">ApiKey to validate.</param>
    /// <param name="instance">The RedCap instance to try.</param>
    /// <returns>The matching study from RedCap</returns>
    /// <exception cref="UnauthorizedAccessException">The APIKey is not authorized with RedCap.</exception>
    private async Task<StudyModel> ValidateWithInstance(string apiKey, string instance)
    {
        var result = await redCapStudyService.GetStudy(instance, apiKey);
        result.ApiKey = apiKey;
        result.Instance = instance;
        return result;
    }
    
}
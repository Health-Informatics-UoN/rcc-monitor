using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Models;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Data.Entities;

namespace Monitor.Services;

public class StudyService
{
    private readonly ApplicationDbContext _db;
    private readonly RedCapOptions _config;
    private const string StudiesUrl = "/rest/v2/studies";
    
    public StudyService(ApplicationDbContext db, IOptions<RedCapOptions> config)
    {
        _db = db;
        _config = config.Value;
    }
    
    /// <summary>
    /// Get the list of studies for a relevant user.
    /// </summary>
    /// <param name="userId">User to filter by.</param>
    /// <returns>List of studies.</returns>
    public async Task<IEnumerable<StudyPartialModel>> List(string? userId = null)
    {
        var list = await _db.Studies
            .Include(x => x.Users)
            .Where(x => userId == null || x.Users.Any(s => s.UserId == userId))
            .ToListAsync();
        
        var result = list.Select(x => new StudyPartialModel
        {
            RedCapId = x.RedCapId,
            Name = x.Name
        });
        
        return result;
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
        var check = await _db.Studies.FindAsync(model.Id);
        if (check is not null)
        {
            await AddUser(check.RedCapId, userId);
        }
        else
        {
            var entity = new Study
            {
                ApiKey = model.ApiKey,
                Name = model.Name,
                RedCapId = model.Id,
            };
            _db.Studies.Add(entity);
            
            var user = new StudyUser
            {
                UserId = userId,
                Study = entity,
            };
            _db.StudyUsers.Add(user);
            
            await _db.SaveChangesAsync();

        }
        return model;
    }
    
    /// <summary>
    /// Add a User to an existing Study.
    /// </summary>
    /// <param name="studyId">Study ID to add to.</param>
    /// <param name="userId">UserId to add to it.</param>
    public async Task AddUser(int studyId, string userId)
    {
        var entity = await _db.Studies
            .Where(x => x.RedCapId == studyId)
            .FirstAsync();
        
        var user = new StudyUser
        {
            UserId = userId,
            Study = entity,
        };
        
        _db.StudyUsers.Add(user);
        await _db.SaveChangesAsync();
    }
    
    /// <summary>
    /// Validate the Study's APIKey against RedCap.
    /// </summary>
    /// <param name="apiKey">APIKey to validate.</param>
    /// <returns>The study attached to the APIKey.</returns>
    /// <exception cref="UnauthorizedAccessException">The APIKey is not authorized with RedCap.</exception>
    /// <exception cref="Exception">We failed to reach RedCap.</exception>
    public async Task<StudyModel> Validate(string apiKey)
    {
        try
        {
            var url = _config.ApiUrl + StudiesUrl;
            var result = await url.WithHeader("token", apiKey).GetJsonAsync<StudyModel>();
            result.ApiKey = apiKey;
            return result;
        }
        catch (FlurlHttpException ex)
        {
            if (ex.Call.Response.StatusCode == 401)
            {
                throw new UnauthorizedAccessException("API Key is unauthorized to access RedCap.");
            }
            else
            {
                throw new Exception("We could not reach RedCap to validate the API Key.");
            }
        }
    }

}
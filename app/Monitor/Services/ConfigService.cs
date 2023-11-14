using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Models;

namespace Monitor.Services;

public class ConfigService
{
    private readonly ApplicationDbContext _db;

    public ConfigService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Get the list of config.
    /// </summary>
    /// <returns>List of config.</returns>
    public async Task<IEnumerable<ConfigModel>> List()
    {
        var list = await _db.Config.ToListAsync();
        
        var result = list.Select(x => new ConfigModel
        {
            Key = x.Key,
            Value = x.Value,
            Name = x.Name,
            Description = x.Description
        });

        return result;
    }

    /// <summary>
    /// Edits a Config.
    /// </summary>
    /// <param name="key">Key of the config to update.</param>
    /// <param name="configModel">New model to update with.</param>
    /// <exception cref="KeyNotFoundException">Config not found.</exception>
    public async Task Edit(string key, ConfigModel configModel)
    {
        var entity = await _db.Config.FirstOrDefaultAsync(x => x.Key == key)
                     ?? throw new KeyNotFoundException($"Config with key \"{key}\" not found");
        
        entity.Value = configModel.Value;
        entity.Name = configModel.Name;
        entity.Description = configModel.Description;
        
        await _db.SaveChangesAsync();
    }
}
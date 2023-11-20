using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Data.Entities;
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
            Description = x.Description,
            Type = x.Type
        });

        return result;
    }

    /// <summary>
    /// Edits a Config.
    /// </summary>
    /// <param name="configModel">New model to update with.</param>
    /// <exception cref="KeyNotFoundException">Config not found.</exception>
    /// <exception cref="ArgumentException">Value not valid.</exception>
    public async Task Edit(UpdateConfigModel configModel)
    {
        var entity = await _db.Config.FirstOrDefaultAsync(x => x.Key == configModel.Key)
                     ?? throw new KeyNotFoundException($"Config with key \"{configModel.Key}\" not found");
        
        var configType = entity.Type;
        
        switch (configType)
        {
            case ConfigType.TimeSpan:
                if (!TimeSpan.TryParse(configModel.Value, out _))
                {
                    throw new ArgumentException("The value provided is not valid");
                }
                break;
            
            case ConfigType.Double:
                if (!double.TryParse(configModel.Value, out double value) || value < 0 || value > 100)
                {
                    throw new ArgumentException("The value provided is not valid");
                }
                break;
            
            default:
                throw new ArgumentException($"The value provided is not valid");
        }
        entity.Value = configModel.Value;
        await _db.SaveChangesAsync();
    }
}
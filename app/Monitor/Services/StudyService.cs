using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Models;

namespace Monitor.Services;

public class StudyService
{
    private readonly ApplicationDbContext _db;

    public StudyService(ApplicationDbContext db)
    {
        _db = db;
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

}
using Data.Constants;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _db;

    public DataSeeder(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task SeedReportTypes()
    {
        if (!await _db.ReportTypes
                .AsNoTracking()
                .AnyAsync())
        {
            var seedReportTypes = new List<ReportType>
            {
                new ReportType()
                {
                    Name =  Reports.MismatchingSites,
                },                new ReportType()
                {
                    Name =  Reports.MismatchingSiteName,
                },                new ReportType()
                {
                    Name =  Reports.MismatchingSiteParent,
                },
            };
            foreach (var s in seedReportTypes)
            {
                _db.Add(s);

            }
            await _db.SaveChangesAsync();

        }
    }
}

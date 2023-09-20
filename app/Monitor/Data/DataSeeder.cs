using Data.Constants;
using Microsoft.EntityFrameworkCore;
using Monitor.Data.Entities;

namespace Monitor.Data;

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
          Name =  Reports.ConflictingSites,
        },                new ReportType()
        {
          Name =  Reports.ConflictingSiteName,
        },                new ReportType()
        {
          Name =  Reports.ConflictingSiteParent,
        },
      };
      foreach (var s in seedReportTypes)
      {
        _db.Add(s);

      }
      await _db.SaveChangesAsync();

    }
  }
  public async Task SeedInstanceTypes()
  {
    if (!await _db.Instances
          .AsNoTracking()
          .AnyAsync())
    {
      var seedInstances = new List<Instance>
      {
        new Instance()
        {
          Name =  Instances.Build,
        },
        new Instance()
        {
          Name =  Instances.Uat,
        },
        new Instance()
        {
          Name =  Instances.Production,
        }
      };
      foreach (var s in seedInstances)
      {
        _db.Add(s);

      }
      await _db.SaveChangesAsync();

    }
  }
  
  public async Task SeedReportStatus()
  {
    if (!await _db.ReportStatus
          .AsNoTracking()
          .AnyAsync())
    {
      var seedStatus = new List<ReportStatus>
      {
        new ReportStatus()
        {
          Name =  Status.Active,
        },
        new ReportStatus()
        {
          Name =  Status.Resolved,
        },
        new ReportStatus()
        {
          Name =  Status.Viewed,
        }
      };
      foreach (var s in seedStatus)
      {
        _db.Add(s);

      }
      await _db.SaveChangesAsync();

    }
  }
}

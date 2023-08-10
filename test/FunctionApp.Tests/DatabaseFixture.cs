using Microsoft.EntityFrameworkCore;
using Monitor.Data;

namespace Francois.Tests;

public class DatabaseFixture
{
    public readonly ApplicationDbContext DbContext;
    
    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        DbContext = new ApplicationDbContext(options);
    }

    public async Task SeedTestData()
    {
        var seeder = new DataSeeder(DbContext);
        await seeder.SeedReportTypes();
        await seeder.SeedReportStatus();
        await seeder.SeedInstanceTypes();
    }
    
}

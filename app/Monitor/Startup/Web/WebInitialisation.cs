using Monitor.Data;

namespace Monitor.Startup.Web;

public static class WebInitialisation
{
    /// <summary>
    ///  Do any app initialisation after the the app has been built (and DI services are locked down)
    ///
    /// e.g. Internal Data Seeding on App Startup
    /// </summary>
    /// <param name="app"></param>
    public static async Task Initialise(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var seeder = new DataSeeder(db);

        await seeder.SeedReportTypes();
        await seeder.SeedInstanceTypes();
        await seeder.SeedReportStatus();
        await seeder.SeedConfig();
    }
    
}
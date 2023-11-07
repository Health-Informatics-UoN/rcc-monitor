using Keycloak.AuthServices.Sdk.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Data;
using Monitor.Services;
using Moq;

namespace Francois.Tests;

public class Fixtures
{
    public readonly ApplicationDbContext DbContext;
    
    public Fixtures()
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
    
    /// <summary>
    /// Fixture for the Study Service.
    /// </summary>
    /// <returns></returns>
    public StudyService GetStudyService()
    {
        var options = Options.Create(new RedCapOptions());
        var mockKeycloakUserClient = new Mock<IKeycloakUserClient>();
        var userService = new UserService(DbContext, mockKeycloakUserClient.Object, Options.Create(new KeycloakOptions()));

        return new StudyService(DbContext, options, userService);
    }
    
}

using Functions.Services;
using Keycloak.AuthServices.Sdk.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Data;
using Monitor.Data.Config;
using Monitor.Services;
using Monitor.Shared.Config;
using Monitor.Shared.Constants;
using Monitor.Shared.Models.Studies;
using Monitor.Shared.Services;
using Moq;

namespace Monitor.Tests;

public class Fixtures
{
    public readonly ApplicationDbContext DbContext;
    
    public Fixtures() 
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var encryptionOptions = Options.Create(new EncryptionOptions()
        {
            EncryptionKey = "test-key-example"
        });
        
        DbContext = new ApplicationDbContext(options, encryptionOptions);
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
        var configService = new ConfigService(DbContext);
        var authorizationService = new Mock<IAuthorizationService>().Object;
        var redCapStudyService = new Mock<IRedCapStudyService>();
        
        // Mock the RedCap API to return AuditLogs and StudyGroups read permissions.
        var expectedStudyRole = new StudyRole
        {
            permissions =
            [
                new StudyRoleComponentPermissions
                {
                    ComponentName = ComponentName.AuditLogs,
                    AllowedPermissions =
                    [
                        new GenericItem
                        {
                            Id = 1,
                            Name = AllowedPermissions.ResourcePermissionRead
                        },
                        new GenericItem
                        {
                            Id = 2,
                            Name = AllowedPermissions.ResourcePermissionDelete
                        }
                    ],
                    Permissions = [1]
                },
                new StudyRoleComponentPermissions
                {
                    ComponentName = ComponentName.StudyGroups,
                    AllowedPermissions =
                    [
                        new GenericItem
                        {
                            Id = 1,
                            Name = AllowedPermissions.ResourcePermissionRead
                        },
                        new GenericItem
                        {
                            Id = 2,
                            Name = AllowedPermissions.ResourcePermissionDelete
                        }
                    ],
                    Permissions = [1]
                }
            ]
        };
        redCapStudyService.Setup(x => x.GetStudyRole(It.IsAny<StudyModel>(), It.IsAny<int>()))
            .ReturnsAsync(expectedStudyRole);

        return new StudyService(DbContext, options, userService, configService, authorizationService, redCapStudyService.Object);
    }

    /// <summary>
    /// Fixture for the Study Capacity Service.
    /// </summary>
    /// <returns></returns>
    public StudyCapacityService GetStudyCapacityService()
    {
        var redCapStudyService = new RedCapStudyService(Options.Create(new RedCapOptions()));

        return new StudyCapacityService(DbContext, redCapStudyService);
    }
    
}

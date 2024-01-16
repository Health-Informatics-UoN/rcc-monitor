using Monitor.Shared.Constants;
using Monitor.Shared.Models.Studies;

namespace Monitor.Tests.Services;

public class StudyPermissionsServiceTests(Fixtures fixtures) : IClassFixture<Fixtures>
{
    [Fact]
    public void CheckPermissions_AllRequiredPermissionsPresent_ReturnsTrue()
    {
        // Arrange
        var studyService = fixtures.GetStudyService();
        var permission = new StudyRoleComponentPermissions
        {
            AllowedPermissions = [new GenericItem { Id = 1, Name = AllowedPermissions.ResourcePermissionRead }],
            Permissions = [1]
        };

        var allowedPermissions = new List<string> { AllowedPermissions.ResourcePermissionRead };

        // Act
        var result = studyService.CheckPermissions(permission, allowedPermissions);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void CheckPermissions_RequiredPermissionsMissing_ReturnsFalse()
    {
        // Arrange
        var studyService = fixtures.GetStudyService();
        var permission = new StudyRoleComponentPermissions
        {
            AllowedPermissions = [new GenericItem { Id = 1, Name = AllowedPermissions.ResourcePermissionRead }],
            Permissions = [1]
        };

        var allowedPermissions = new List<string> { 
            AllowedPermissions.ResourcePermissionRead, 
            AllowedPermissions.ResourcePermissionDelete 
        };

        // Act
        var result = studyService.CheckPermissions(permission, allowedPermissions);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void CheckPermissions_ExistingPermissionsWrong_ReturnsFalse()
    {
        // Arrange
        var studyService = fixtures.GetStudyService();
        var permission = new StudyRoleComponentPermissions
        {
            AllowedPermissions = [new GenericItem { Id = 1, Name = AllowedPermissions.ResourcePermissionDelete }],
            Permissions = [1]
        };

        var allowedPermissions = new List<string> { 
            AllowedPermissions.ResourcePermissionRead, 
        };

        // Act
        var result = studyService.CheckPermissions(permission, allowedPermissions);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void CheckPermissions_ExistingPermissionsAreExtra_ReturnsFalse()
    {
        // Arrange
        var studyService = fixtures.GetStudyService();
        var permission = new StudyRoleComponentPermissions
        {
            AllowedPermissions =
            [
                new GenericItem { Id = 1, Name = AllowedPermissions.ResourcePermissionDelete },
                new GenericItem { Id = 2, Name = AllowedPermissions.ResourcePermissionRead }
            ],
            Permissions = [1, 2]
        };

        var allowedPermissions = new List<string> { 
            AllowedPermissions.ResourcePermissionRead
        };

        // Act
        var result = studyService.CheckPermissions(permission, allowedPermissions);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(ComponentName.AuditLogs, new[] {AllowedPermissions.ResourcePermissionRead}, ComponentName.StudyGroups)]
    public async Task UnwrapPermissions_ReturnsCorrectPermissions(string componentName, string[] allowedPermissions, string additionalPermissions)
    {
        // Arrange
        var studyService = fixtures.GetStudyService();

        var study = new StudyModel();

        var studyUserAssignments = new List<StudyAssignment>
        {
            new()
            {
                roleId = 1
            }
        };
        
        var requiredPermissions = new Dictionary<string, List<string>>
        {
            { componentName, allowedPermissions.ToList() }
        };
        
        // Act
        var (permissionsResult, extraPermissions) = 
            await studyService.UnwrapPermissions(studyUserAssignments, requiredPermissions, study);
        
        // Assert
        Assert.True(permissionsResult.ContainsKey(componentName));
        Assert.True(permissionsResult[componentName]);
        Assert.Contains(additionalPermissions, extraPermissions);

    }
    
}
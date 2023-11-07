using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Groups;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Options;
using Monitor.Config;
using Monitor.Constants;
using Monitor.Data.Entities;
using Monitor.Models.Users;
using Monitor.Services;
using Moq;

namespace Francois.Tests.Services;

public class UserServiceTests : IClassFixture<Fixtures>
{
    private readonly Fixtures _fixtures;
    private readonly UserService _userService;
    private readonly Mock<IKeycloakUserClient> _mockKeycloakUserClient;

    public UserServiceTests(Fixtures fixtures)
    {
        _fixtures = fixtures;
        _mockKeycloakUserClient = new Mock<IKeycloakUserClient>();
        _userService = new UserService(_fixtures.DbContext, _mockKeycloakUserClient.Object,
            Options.Create(new KeycloakOptions()));
    }

    [Fact]
    public async Task GetUnaffiliatedUsers_ShouldReturnUnaffiliatedUsers()
    {
        // Arrange
        var unaffiliatedQuery = new UnaffiliatedQuery
        {
            Query = "test",
            StudyId = 4
        };

        // Mock GetUsers to return a list of keycloak users.
        var mockUsers = new List<User>
        {
            new() { Id = "1", Email = "test@example.com" },
            new() { Id = "2", Email = "test2@example.com" },
        };
        _mockKeycloakUserClient
            .Setup(client => client.GetUsers(It.IsAny<string>(), It.IsAny<GetUsersRequestParameters>()))
            .ReturnsAsync(mockUsers);

        // Mock GetUserGroups to always be a trial manager.
        var mockGroups = new List<Group>
        {
            new() { Name = "TrialManager" }
        };
        _mockKeycloakUserClient.Setup(client =>
                client.GetUserGroups(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetGroupRequestParameters>()))
            .ReturnsAsync(mockGroups);

        // Add a study with no users attached
        var sampleStudy = new Study { RedCapId = 4 };
        _fixtures.DbContext.Studies.Add(sampleStudy);
        await _fixtures.DbContext.SaveChangesAsync();

        // Act
        var unaffiliatedUsers = await _userService.GetUnaffiliatedUsers(unaffiliatedQuery);

        // Assert
        Assert.Contains(unaffiliatedUsers, u => u.Id == "1" && u.Email == "test@example.com");
        Assert.Contains(unaffiliatedUsers, u => u.Id == "2" && u.Email == "test2@example.com");
    }

    [Fact]
    public async Task GetUnaffiliatedUsers_ShouldNotReturnAffiliatedUsers()
    {
        // Arrange
        var unaffiliatedQuery = new UnaffiliatedQuery
        {
            Query = "test",
            StudyId = 5
        };

        // Mock GetUsers to return a list of keycloak users
        var mockUsers = new List<User>
        {
            new() { Id = "3", Email = "test3@example.com" }
        };
        _mockKeycloakUserClient
            .Setup(client => client.GetUsers(It.IsAny<string>(), It.IsAny<GetUsersRequestParameters>()))
            .ReturnsAsync(mockUsers);

        // Mock GetUserGroups to always be a trial manager.
        var mockGroups = new List<Group>
        {
            new() { Name = Groups.TrialManager }
        };
        _mockKeycloakUserClient.Setup(client =>
                client.GetUserGroups(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetGroupRequestParameters>()))
            .ReturnsAsync(mockGroups);

        // Add a study with users attached
        var sampleStudy = new Study { RedCapId = 5, Users = new List<StudyUser> { new() { UserId = "3" } } };
        _fixtures.DbContext.Studies.Add(sampleStudy);
        await _fixtures.DbContext.SaveChangesAsync();

        // Act
        var unaffiliatedUsers = await _userService.GetUnaffiliatedUsers(unaffiliatedQuery);

        // Assert
        Assert.DoesNotContain(unaffiliatedUsers, u => u.Id == "3");
    }

    [Fact]
    public async Task GetUnaffiliatedUsers_ShouldNotReturnAdminUsers()
    {
        // Arrange
        var unaffiliatedQuery = new UnaffiliatedQuery
        {
            Query = "test",
            StudyId = 6
        };

        // Mock GetUsers to return a list of keycloak users
        var mockUsers = new List<User>
        {
            new() { Id = "4", Email = "test4@example.com" }
        };
        _mockKeycloakUserClient
            .Setup(client => client.GetUsers(It.IsAny<string>(), It.IsAny<GetUsersRequestParameters>()))
            .ReturnsAsync(mockUsers);

        // Mock GetUserGroups to always be an admin.
        var mockGroups = new List<Group>
        {
            new() { Name = Groups.Admin }
        };
        _mockKeycloakUserClient.Setup(client =>
                client.GetUserGroups(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetGroupRequestParameters>()))
            .ReturnsAsync(mockGroups);

        // Add a study with no users attached
        var sampleStudy = new Study { RedCapId = 6 };
        _fixtures.DbContext.Studies.Add(sampleStudy);
        await _fixtures.DbContext.SaveChangesAsync();

        // Act
        var unaffiliatedUsers = await _userService.GetUnaffiliatedUsers(unaffiliatedQuery);

        // Assert
        Assert.DoesNotContain(unaffiliatedUsers, u => u.Id == "4");
    }

    [Fact]
    public async Task GetUnaffiliatedUsers_ShouldReturnTrialManagerUsers()
    {
        // Arrange
        var unaffiliatedQuery = new UnaffiliatedQuery
        {
            Query = "test",
            StudyId = 7
        };

        // Mock GetUsers to return a list of keycloak users
        var mockUsers = new List<User>
        {
            new() { Id = "5", Email = "test5@example.com" }
        };
        _mockKeycloakUserClient
            .Setup(client => client.GetUsers(It.IsAny<string>(), It.IsAny<GetUsersRequestParameters>()))
            .ReturnsAsync(mockUsers);

        // Mock GetUserGroups to always be an admin.
        var mockGroups = new List<Group>
        {
            new() { Name = Groups.TrialManager }
        };
        _mockKeycloakUserClient.Setup(client =>
                client.GetUserGroups(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetGroupRequestParameters>()))
            .ReturnsAsync(mockGroups);

        // Add a study with no users attached
        var sampleStudy = new Study { RedCapId = 7 };
        _fixtures.DbContext.Studies.Add(sampleStudy);
        await _fixtures.DbContext.SaveChangesAsync();

        // Act
        var unaffiliatedUsers = await _userService.GetUnaffiliatedUsers(unaffiliatedQuery);

        // Assert
        Assert.Contains(unaffiliatedUsers, u => u.Id == "5");
    }
}
using Microsoft.EntityFrameworkCore;
using Monitor.Data.Entities;
using Monitor.Shared.Models.Studies;
using StudyUser = Monitor.Data.Entities.StudyUser;

namespace Monitor.Tests.Services;

public class StudyServiceTests(Fixtures fixtures) : IClassFixture<Fixtures>
{
    [Fact]
    public async Task AddUser_ShouldAddUserToStudy()
    {
        // Arrange
        var context = fixtures.DbContext;
        var studyService = fixtures.GetStudyService();

        var study = new Study
        {
            RedCapId = 1, 
            ApiKey = "test"
        };
        context.Studies.Add(study);
        await context.SaveChangesAsync();

        var userId = "testUserId"; 

        // Act
        await studyService.AddUser(study.RedCapId, userId);

        // Assert
        var addedUser = await context.StudyUsers
            .Where(u => u.UserId == userId && u.Study.RedCapId == study.RedCapId)
            .Include(studyUser => studyUser.Study)
            .FirstOrDefaultAsync();

        // Assert that the user was added
        Assert.NotNull(addedUser);
        Assert.Equal(userId, addedUser?.UserId);
        Assert.Equal(study.RedCapId, addedUser?.Study.RedCapId);
    }
    
    [Fact]
    public async Task RemoveUsers_ShouldRemoveUsersFromStudy()
    {
        // Arrange
        var context = fixtures.DbContext;
        var studyService = fixtures.GetStudyService();
        
        var study = new Study
        {
            RedCapId = 2,
            ApiKey = "test",
            Users = new List<StudyUser>
            {
                new() { UserId = "user1" },
                new() { UserId = "user2" },
                new() { UserId = "user3" }
            }
        };
        context.Studies.Add(study);
        await context.SaveChangesAsync();

        // Input with the users who are kept.
        var model = new StudyPartialModel
        {
            Users = new List<Shared.Models.Studies.StudyUser>
            {
                new() { Id = "user1" },
                new() { Id = "user2" }
            }
        };

        // Act
        await studyService.RemoveUsers(study.RedCapId, model);

        // Assert
        var remainingUsers = await context.StudyUsers
            .Where(u => u.Study.RedCapId == study.RedCapId)
            .ToListAsync();

        // Check that the specified users were removed
        Assert.Contains(remainingUsers, u => u.UserId == "user1");
        Assert.Contains(remainingUsers, u => u.UserId == "user2");
        Assert.DoesNotContain(remainingUsers, u => u.UserId == "user3");
    }
    
    [Fact]
    public async Task AddUsers_ShouldAddUsersToStudy()
    {
        // Arrange
        var context = fixtures.DbContext;
        var studyService = fixtures.GetStudyService();

        var study = new Study
        {
            RedCapId = 3,
            ApiKey = "test",
            Users = new List<StudyUser>
            {
                new() { UserId = "user1" },
                new() { UserId = "user2" },
            }
        };
        context.Studies.Add(study);
        await context.SaveChangesAsync();

        // Input with new users to be added
        var model = new StudyPartialModel
        {
            Users = new List<Shared.Models.Studies.StudyUser>
            {
                new() { Id = "user3" },
                new() { Id = "user4" }
            }
        };

        // Act
        await studyService.AddUsers(study.RedCapId, model);

        // Assert
        var updatedStudy = await context.Studies
            .Include(s => s.Users)
            .Where(s => s.RedCapId == study.RedCapId)
            .FirstOrDefaultAsync();

        // Check that the users were added
        Assert.NotNull(updatedStudy?.Users.FirstOrDefault(u => u.UserId == "user3"));
        Assert.NotNull(updatedStudy?.Users.FirstOrDefault(u => u.UserId == "user4"));
    }
    
}
using BackendClassLib.Database;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;

namespace BackendClassLib.UnitTests;

public class BugRepositoryTests
{
    [Fact]
    public async Task Create_Bug_ThrowsProjectNotFoundException()
    {
        // Arrange
        List<Project> projects = [];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(c => c.Projects).Returns(mockProjectsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await bugRepository.CreateBugAsync(0, 0, "A", string.Empty));
    }

    [Fact]
    public async Task Create_Bug_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        List<Project> projects =
        [
            new Project
            {
                Id = 1,
                Name = "A",
            }
        ];
        List<User> users = [];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();
        Mock<DbSet<User>> mockUsersSet = users.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsersSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await bugRepository.CreateBugAsync(1, 0, "A", string.Empty));
    }

    [Fact]
    public async Task Create_Bug_SuccessfullySaved()
    {
        // Arrange
        List<Project> projects =
        [
            new Project
            {
                Id = 1,
                Name = "A"
            }
        ];
        List<User> users =
        [
            new User
            {
                Id = 1,
                DisplayName = "Testing User 1"
            }
        ];
        projects[0].Users.Add(users[0]);

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();
        Mock<DbSet<User>> mockUsersSet = users.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsersSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        await bugRepository.CreateBugAsync(1, 1, "A", null);

        // Assert
        Assert.True(mockContext.Object.Projects.Any(c => c.Bugs.Count == 1));
        Assert.True(mockContext.Object.Projects.Any(c => c.Bugs.Where(x => x.Title == "A").Any()));
    }

    [Fact]
    public async Task Get_Bugs_ThrowsProjectNotFoundException()
    {
        // Arrange
        List<Project> projects = [];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await bugRepository.GetBugsAsync(0, 0));
    }

    [Fact]
    public async Task Get_Bugs_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        List<Project> projects =
        [
            new Project
            {
                Id = 1,
                Name = "A"
            }
        ];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await bugRepository.GetBugsAsync(1, 0));
    }

    [Fact]
    public async Task Get_Bugs_ReturnsBugs()
    {
        // Arrange
        List<User> users =
        [
            new User
            {
                Id = 1,
                DisplayName = "Testing User 1"
            }
        ];

        List<Project> projects =
        [
            new Project
            {
                Id = 1,
                Name = "A"
            }
        ];

        projects[0].Users.Add(users[0]);

        projects[0].Bugs.Add(new Bug
        {
            Id = 1,
            Title = "A"
        });
        projects[0].Bugs.Add(new Bug
        {
            Id = 2,
            Title = "B"
        });
        projects[0].Bugs.Add(new Bug
        {
            Id = 3,
            Title = "C"
        });

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();
        Mock<DbSet<User>> mockUsersSet = users.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsersSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Act
        List<Bug> bugs = await bugRepository.GetBugsAsync(1, 1);

        // Assert
        Assert.Equal(3, bugs.Count);
    }

    [Fact]
    public async Task Delete_Bug_ThrowsProjectNotFoundException()
    {
        // Arrange
        List<Project> projects = [];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await bugRepository.DeleteBugAsync(0, 0, 0));
    }

    [Fact]
    public async Task Delete_Bug_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        List<Project> projects =
        [
            new()
            {
                Id = 1,
                Name = "A"
            }
        ];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await bugRepository.DeleteBugAsync(1, 0, 0));
    }

    [Fact]
    public async Task Delete_Bug_ThrowsInsufficientPermissionToDeleteBugException()
    {
        // Arrange
        List<Project> projects =
        [
            new()
            {
                Id = 1,
                Name = "A"
            }
        ];

        List<User> users =
        [
            new()
            {
                Id = 1,
                DisplayName = "Testing User 1"
            }
        ];
        projects[0].Users.Add(users[0]);

        List<Bug> bugs =
        [
            new()
            {
                Id = 1,
                Title = "A"
            }
        ];
        bugs[0].ProjectId = 1;

        List<ProjectPermission> projectPermissions = [];

        List<UserProjectPermission> userProjectPermissions = [];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();
        Mock<DbSet<User>> mockUsersSet = users.BuildMockDbSet();
        Mock<DbSet<Bug>> mockBugsSet = bugs.BuildMockDbSet();
        Mock<DbSet<ProjectPermission>> mockProjectPermissionsSet = projectPermissions.BuildMockDbSet();
        Mock<DbSet<UserProjectPermission>> mockUserProjectPermissionsSet = userProjectPermissions.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsersSet.Object);
        mockContext.Setup(x => x.Bugs).Returns(mockBugsSet.Object);
        mockContext.Setup(x => x.ProjectPermissions).Returns(mockProjectPermissionsSet.Object);
        mockContext.Setup(x => x.UserProjectPermissions).Returns(mockUserProjectPermissionsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<InsufficientPermissionToDeleteBugException>(async () => await bugRepository.DeleteBugAsync(1, 1, 1));
    }

    [Fact]
    public async Task Delete_Bug_ThrowsBugNotFoundException()
    {
        // Arrange
        List<User> users =
        [
            new()
            {
                Id = 1,
                DisplayName = "Testing User 1"
            }
        ];

        List<Project> projects =
        [
            new()
            {
                Id = 1,
                Name = "A"
            }
        ];
        projects[0].Users.Add(users[0]);

        List<Bug> bugs = [];

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();
        Mock<DbSet<User>> mockUsersSet = users.BuildMockDbSet();
        Mock<DbSet<Bug>> mockBugsSet = bugs.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsersSet.Object);
        mockContext.Setup(x => x.Bugs).Returns(mockBugsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<BugNotFoundException>(async () => await bugRepository.DeleteBugAsync(1, 1, 0));
    }

    [Fact]
    public async Task Delete_Bug_ThrowsNotProjectBugException()
    {
        // Arrange
        List<User> users =
        [
            new()
            {
                Id = 1,
                DisplayName = "Testing User 1"
            }
        ];

        List<Project> projects =
        [
            new()
            {
                Id = 1,
                Name = "A"
            },
            new()
            {
                Id = 2,
                Name = "B"
            }
        ];
        projects[0].Users.Add(users[0]);

        List<Bug> bugs =
        [
            new()
            {
                Id = 1,
                Title = "A"
            }
        ];
        projects[1].Bugs.Add(bugs[0]);

        Mock<DbSet<Project>> mockProjectsSet = projects.BuildMockDbSet();
        Mock<DbSet<User>> mockUsersSet = users.BuildMockDbSet();
        Mock<DbSet<Bug>> mockBugsSet = bugs.BuildMockDbSet();

        Mock<ApplicationDbContext> mockContext = new();
        mockContext.Setup(x => x.Projects).Returns(mockProjectsSet.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsersSet.Object);
        mockContext.Setup(x => x.Bugs).Returns(mockBugsSet.Object);

        BugRepository bugRepository = new(mockContext.Object);

        // Assert
        await Assert.ThrowsAsync<NotProjectBugException>(async () => await bugRepository.DeleteBugAsync(1, 1, 1));
    }

    [Fact]
    public async Task Delete_Bug_DeletedSuccessfully()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext context = new(contextOptions);
        await context.Database.EnsureCreatedAsync();

        Auth testingUserAuth = new()
        {
            UserIds = ["auth0|4w8rgord1q37xtib036wo0km"]
        };

        User testingUser = new()
        {
            DisplayName = "Testing User",
            Auth = testingUserAuth,
        };
        await context.AddAsync(testingUser);

        Project testProject = new()
        {
            Name = "Test"
        };
        testProject.Users.Add(testingUser);

        Bug testBug = new()
        {
            Title = "Test",
            Project = testProject
        };

        await context.AddAsync(testBug);

        ProjectPermission deleteBugPermission = new()
        {
            Name = "Delete Bug",
            Description = "The permission to delete a bug from a project.",
            Type = ProjectPermissionType.DeleteBug
        };

        await context.AddAsync(new UserProjectPermission()
        {
            Project = testProject,
            User = testingUser,
            ProjectPermission = deleteBugPermission
        });

        await context.SaveChangesAsync();

        BugRepository bugRepository = new(context);

        // Act
        await bugRepository.DeleteBugAsync(testProject.Id, testingUser.Id, testBug.Id);

        // Assert
        Assert.Null(await context.Bugs.FindAsync(testBug.Id));

        await connection.CloseAsync();
    }


    [Fact]
    public async Task MarkBugAsAssigned_NonExistingBugId_ThrowsBugNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        const int BugId = 0;
        const int ProjectId = 0;
        const int UserId = 0;

        // Act
        async Task actual() => await bugRepository.MarkBugAsAssigned(BugId, ProjectId, UserId);

        // Assert
        await Assert.ThrowsAsync<BugNotFoundException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task MarkBugAsAssigned_NonExistingProjectId_ThrowsProjectNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        Bug testBug1 = new()
        {
            Title = "Test Bug 1",
            Status = BugStatusType.New
        };

        Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        testProject1.Bugs.Add(testBug1);

        await applicationDbContext.AddAsync(testProject1);
        await applicationDbContext.SaveChangesAsync();

        const int ProjectId = 0;
        const int UserId = 0;

        // Act
        async Task actual() => await bugRepository.MarkBugAsAssigned(testBug1.Id, ProjectId, UserId);

        // Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task MarkBugAsAssigned_NonExistingUserId_ThrowsUserNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        Bug testBug1 = new()
        {
            Title = "Test Bug 1",
            Status = BugStatusType.New
        };

        Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        testProject1.Bugs.Add(testBug1);

        await applicationDbContext.AddAsync(testProject1);
        await applicationDbContext.SaveChangesAsync();

        const int UserId = 0;

        // Act
        async Task actual() => await bugRepository.MarkBugAsAssigned(testBug1.Id, testProject1.Id, UserId);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task MarkBugAsAssigned_NonProjectUser_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        Bug testBug1 = new()
        {
            Title = "Test Bug 1",
            Status = BugStatusType.New
        };

        Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        testProject1.Bugs.Add(testBug1);

        User testUser1 = new()
        {
            DisplayName = "Test User 1",
            Auth = new()
            {
                UserIds = ["auth0|0f71ki6ovb1wd3cgsx1rm5qm"]
            }
        };

        await applicationDbContext.AddRangeAsync([testProject1, testUser1]);
        await applicationDbContext.SaveChangesAsync();

        // Act
        async Task actual() => await bugRepository.MarkBugAsAssigned(testBug1.Id, testProject1.Id, testUser1.Id);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task MarkBugAsAssigned_NonProjectBug_ThrowsNotProjectBugException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        Bug testBug1 = new()
        {
            Title = "Test Bug 1",
            Status = BugStatusType.New
        };

        Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        testProject1.Bugs.Add(testBug1);

        User testUser1 = new()
        {
            DisplayName = "Test User 1",
            Auth = new()
            {
                UserIds = ["auth0|0f71ki6ovb1wd3cgsx1rm5qm"]
            }
        };

        await applicationDbContext.AddRangeAsync([testProject1, testUser1]);
        await applicationDbContext.SaveChangesAsync();

        // Act
        async Task actual() => await bugRepository.MarkBugAsAssigned(testBug1.Id, testProject1.Id, testUser1.Id);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task MarkBugAsAssigned_Status_Updated()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        Bug testBug1 = new()
        {
            Title = "Test Bug 1",
            Status = BugStatusType.New
        };

        Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        testProject1.Bugs.Add(testBug1);

        User testUser1 = new()
        {
            DisplayName = "Test User 1",
            Auth = new()
            {
                UserIds = ["auth0|0f71ki6ovb1wd3cgsx1rm5qm"]
            }
        };
        testUser1.Projects.Add(testProject1);

        await applicationDbContext.AddRangeAsync([testProject1, testUser1]);
        await applicationDbContext.SaveChangesAsync();

        // Act
        await bugRepository.MarkBugAsAssigned(testBug1.Id, testProject1.Id, testUser1.Id);

        // Assert
        Assert.Equal(BugStatusType.Assigned, testBug1.Status);
    }

    [Fact]
    public async Task AssignCollaboratorToBugAsync_BugNotFound_ThrowsBugNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        BugRepository bugRepository = new(applicationDbContext);

        const int BugId = 0;
        const int UserId = 0;
        const int AssigneeUserId = 0;

        // Act
        async Task actual() => await bugRepository.AssignCollaboratorToBugAsync(BugId, UserId, AssigneeUserId);

        // Assert
        await Assert.ThrowsAsync<BugNotFoundException>(actual);
    }

    [Fact]
    public async Task AssignCollaboratorToBugAsync_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Bug testBug1 = new()
        {
            Title = "Test Bug 1",
            Status = BugStatusType.New,
            Project = new()
            {
                Name = "Test Project 1"
            }
        };

        await applicationDbContext.AddRangeAsync([testBug1]);
        await applicationDbContext.SaveChangesAsync();

        BugRepository bugRepository = new(applicationDbContext);

        const int UserId = 0;
        const int AssigneeUserId = 0;

        // Act
        async Task actual() => await bugRepository.AssignCollaboratorToBugAsync(testBug1.Id, UserId, AssigneeUserId);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(actual);
    }

    [Fact]
    public async Task AssignCollaboratorToBugAsync_UserNotProjectCollaborator_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Bug testBug = new()
        {
            Title = "Test Bug",
            Status = BugStatusType.New,
            Project = new()
            {
                Name = "Test Project"
            }
        };

        const string TestUserAuth0UserId = "auth0|2emcyojaxesfhlf5fr0fxxcd";
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = [TestUserAuth0UserId]
            }
        };

        await applicationDbContext.AddRangeAsync([testBug, testUser]);
        await applicationDbContext.SaveChangesAsync();

        BugRepository bugRepository = new(applicationDbContext);

        const int AssigneeUserId = 0;

        // Act
        async Task actual() => await bugRepository.AssignCollaboratorToBugAsync(testBug.Id, testUser.Id, AssigneeUserId);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(actual);
    }

    [Fact]
    public async Task AssignCollaboratorToBugAsync_AssigneeNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Bug testBug = new()
        {
            Title = "Test Bug",
            Status = BugStatusType.New,
            Project = new()
            {
                Name = "Test Project"
            }
        };

        const string TestUserAuth0UserId = "auth0|2emcyojaxesfhlf5fr0fxxcd";
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = [TestUserAuth0UserId]
            },
        };

        testBug.Project.Users.Add(testUser);

        UserProjectPermission assignCollaboratorToBugUserProjectPermission = new()
        {
            User = testUser,
            Project = testBug.Project,
            ProjectPermission = new()
            {
                Name = "Assign Collaborator To Bug",
                Description = "The permission for a user to be able to assign a collaborator to a bug",
                Type = ProjectPermissionType.AssignCollaboratorToBug
            }
        };

        await applicationDbContext.AddRangeAsync([testBug, testUser, assignCollaboratorToBugUserProjectPermission]);
        await applicationDbContext.SaveChangesAsync();

        BugRepository bugRepository = new(applicationDbContext);

        const int AssigneeUserId = 2;

        // Act
        async Task actual() => await bugRepository.AssignCollaboratorToBugAsync(testBug.Id, testUser.Id, AssigneeUserId);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(actual);
    }

    [Fact]
    public async Task AssignCollaboratorToBugAsync_InsufficientUserProjectPermissionToAssignCollaboratorToBug_ThrowsInsufficientPermissionToAssignCollaboratorToBug()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Bug testBug = new()
        {
            Title = "Test Bug",
            Status = BugStatusType.New,
            Project = new()
            {
                Name = "Test Project"
            }
        };

        const string TestUserAuth0UserId = "auth0|2emcyojaxesfhlf5fr0fxxcd";
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = [TestUserAuth0UserId]
            },
        };

        testBug.Project.Users.Add(testUser);

        await applicationDbContext.AddRangeAsync([testBug, testUser]);
        await applicationDbContext.SaveChangesAsync();

        BugRepository bugRepository = new(applicationDbContext);

        const int AssigneeUserId = 2;

        // Act
        async Task actual() => await bugRepository.AssignCollaboratorToBugAsync(testBug.Id, testUser.Id, AssigneeUserId);

        // Assert
        await Assert.ThrowsAsync<InsufficientPermissionToAssignCollaboratorToBug>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task AssignCollaboratorToBugAsync_SuccessfullyAssignedCollaboratorToBug()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Bug testBug = new()
        {
            Title = "Test Bug",
            Status = BugStatusType.New,
            Project = new()
            {
                Name = "Test Project"
            }
        };

        const string TestUserAuth0UserId = "auth0|2emcyojaxesfhlf5fr0fxxcd";
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = [TestUserAuth0UserId]
            },
        };

        const string TestUser2Auth0UserId = "auth0|tsm7evwof0f02zrjnsvixsco";
        User testUser2 = new()
        {
            DisplayName = "Test User 2",
            Auth = new()
            {
                UserIds = [TestUser2Auth0UserId]
            }
        };

        testBug.Project.Users.Add(testUser);
        testBug.Project.Users.Add(testUser2);

        UserProjectPermission assignCollaboratorToBugUserProjectPermission = new()
        {
            User = testUser,
            Project = testBug.Project,
            ProjectPermission = new()
            {
                Name = "Assign Collaborator To Bug",
                Description = "The permission for a user to be able to assign a collaborator to a bug",
                Type = ProjectPermissionType.AssignCollaboratorToBug
            }
        };

        await applicationDbContext.AddRangeAsync([testBug, testUser, assignCollaboratorToBugUserProjectPermission]);
        await applicationDbContext.SaveChangesAsync();

        BugRepository bugRepository = new(applicationDbContext);

        // Act
        await bugRepository.AssignCollaboratorToBugAsync(testBug.Id, testUser.Id, testUser2.Id);

        // Assert
        bool isTestUser2AssignedBug = await applicationDbContext.Users.AnyAsync(c => c.Id == testUser2.Id && c.AssignedBugs.Contains(testBug));
        Assert.True(isTestUser2AssignedBug);

        await connection.CloseAsync();
    }
}

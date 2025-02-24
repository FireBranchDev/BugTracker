using BackendClassLib.Database;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.UnitTests;

public class ProjectRepositoryTests
{
    [Fact]
    public async Task FindAsync_ProjectNotFound_ThrowsProjectNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int ProjectId = 0;
        const int UserId = 0;

        // Act
        async Task actual() => await projectRepository.FindAsync(ProjectId, UserId);

        // Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task FindAsync_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project"
        };
        await applicationDbContext.AddAsync(testProject);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int UserId = 0;

        // Act
        async Task actual() => await projectRepository.FindAsync(testProject.Id, UserId);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task FindAsync_UserNotProjectCollaborator_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project"
        };
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = ["auth0|on8mgm34ruvyr1xvay126q96"]
            }
        };
        await applicationDbContext.AddRangeAsync([testProject, testUser]);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        // Act
        async Task actual() => await projectRepository.FindAsync(testProject.Id, testUser.Id);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(actual);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task FindAsync_ReturnsProject()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project"
        };
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = ["auth0|on8mgm34ruvyr1xvay126q96"]
            }
        };
        testProject.Users.Add(testUser);
        await applicationDbContext.AddRangeAsync([testProject, testUser]);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        // Act
        Project? foundProject = await projectRepository.FindAsync(testProject.Id, testUser.Id);

        // Assert
        Assert.NotNull(foundProject);
        Assert.Equal(testProject.Id, foundProject.Id);

        await connection.CloseAsync();
    }

    [Fact]
    public async Task DeleteAsync_ProjectNotFound_ThrowsProjectNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int ProjectId = 0;
        const int UserId = 0;

        // Act
        async Task actual() => await projectRepository.DeleteAsync(ProjectId, UserId);

        // Assert
        await Assert.ThrowsAsync<ProjectNotFoundException>(actual);
    }

    [Fact]
    public async Task DeleteAsync_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "Test project's description",
        };

        await applicationDbContext.AddAsync(testProject);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int UserId = 0;

        // Act
        async Task actual() => await projectRepository.DeleteAsync(testProject.Id, UserId);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(actual);
    }

    [Fact]
    public async Task DeleteAsync_UserNotProjectCollaborator_ThrowsUserNotProjectCollaboratorException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "Test project's description",
        };
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = ["auth0|on8mgm34ruvyr1xvay126q96"]
            }
        };
        await applicationDbContext.AddRangeAsync([testProject, testUser]);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        // Act
        async Task actual() => await projectRepository.DeleteAsync(testProject.Id, testUser.Id);

        // Assert
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(actual);
    }

    [Fact]
    public async Task DeleteAsync_InsufficientPermissionToDeleteProject_ThrowsInsufficientPermissionToDeleteProjectException()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "Test project's description",
        };
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = ["auth0|on8mgm34ruvyr1xvay126q96"]
            }
        };
        testProject.Users.Add(testUser);
        await applicationDbContext.AddRangeAsync([testProject, testUser]);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        // Act
        async Task actual() => await projectRepository.DeleteAsync(testProject.Id, testUser.Id);

        // Assert
        await Assert.ThrowsAsync<InsufficientPermissionToDeleteProjectException>(actual);
    }

    [Fact]
    public async Task DeleteAsync_BugSuccessfullyDeleted()
    {
        // Arrange
        SqliteConnection connection = new("Filename=:memory:");
        await connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using ApplicationDbContext applicationDbContext = new(contextOptions);
        await applicationDbContext.Database.EnsureCreatedAsync();

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "Test project's description",
        };
        User testUser = new()
        {
            DisplayName = "Test User",
            Auth = new()
            {
                UserIds = ["auth0|on8mgm34ruvyr1xvay126q96"]
            }
        };
        testProject.Users.Add(testUser);

        UserProjectPermission userProjectPermissionToDeleteProject = new()
        {
            User = testUser,
            Project = testProject,
            ProjectPermission = new()
            {
                Name = "Delete Project",
                Description = "The permission to delete a project.",
                Type = ProjectPermissionType.DeleteProject
            }
        };

        await applicationDbContext.AddRangeAsync([testProject, testUser, userProjectPermissionToDeleteProject]);
        await applicationDbContext.SaveChangesAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        // Act
        await projectRepository.DeleteAsync(testProject.Id, testUser.Id);

        // Assert
        bool deletedTestProjectSuccessfully = !await applicationDbContext.Projects.AnyAsync(c => c.Id == testProject.Id);
        Assert.True(deletedTestProjectSuccessfully);
    }
}

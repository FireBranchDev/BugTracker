using BackendClassLib.Database;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace BackendClassLib.UnitTests
{
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
            await context.SaveChangesAsync();

            BugRepository bugRepository = new(context);

            // Act
            await bugRepository.DeleteBugAsync(testProject.Id, testingUser.Id, testBug.Id);

            // Assert
            Assert.Null(await context.Bugs.FindAsync(testBug.Id));

            await connection.CloseAsync();
        }
    }
}

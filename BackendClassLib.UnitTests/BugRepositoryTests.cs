using BackendClassLib.Database;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib.Exceptions;
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
            IQueryable<Project> data = new List<Project>().AsQueryable();

            Mock<DbSet<Project>> mockSet = new();
            mockSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            Mock<ApplicationDbContext> mockContext = new();
            mockContext.Setup(c => c.Projects).Returns(mockSet.Object);

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
    }
}

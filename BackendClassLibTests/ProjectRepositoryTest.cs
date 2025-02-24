using BackendClassLib.Database;

namespace BackendClassLibTests;

public class ProjectRepositoryTest : BaseDbCollection
{
    [Fact]
    public async Task AddAsync()
    {
        using ApplicationDbContext applicationDbContext = DatabaseFixture.CreateContext();
        await applicationDbContext.Database.BeginTransactionAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const string Name = "Project One";
        const string Description = "Generic description";
        int authId = 1000;

        await Assert.ThrowsAsync<AuthNotFoundException>(async () => await projectRepository.AddAsync(Name, Description, authId));

        const string UserId1 = "auth0|w5u554mylojeaqzufm36bpgv";
        const string UserId2 = "auth0|bmxbr8lmfwscoc7yeom5lqx6";
        Auth auth = new()
        {
            UserIds = [UserId1, UserId2],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        await applicationDbContext.AddAsync(auth);
        await applicationDbContext.SaveChangesAsync();
        authId = auth.Id;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await projectRepository.AddAsync(Name, Description, authId));

        User user = new()
        {
            DisplayName = "John Doe",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            AuthId = authId
        };
        await applicationDbContext.AddAsync(user);
        await applicationDbContext.SaveChangesAsync();

        int insertedProjectId = await projectRepository.AddAsync(Name, Description, authId);
        bool isInsertedProjectSaved = await applicationDbContext.Projects.AnyAsync(c => c.Id == insertedProjectId);
        Assert.True(isInsertedProjectSaved);

        bool isUserInProject = await applicationDbContext.Projects.AnyAsync(c => c.Id == insertedProjectId && c.Users.Contains(user));
        Assert.True(isUserInProject);

        applicationDbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task AddCollaboratorAsync()
    {
        using ApplicationDbContext applicationDbContext = DatabaseFixture.CreateContext();
        await applicationDbContext.Database.BeginTransactionAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int UserIdNotInDb = 1000;
        const int CollaboratorUserIdNotInDb = 1000;
        const int ProjectIdNotInDb = 1000;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await projectRepository.AddCollaboratorAsync(UserIdNotInDb, CollaboratorUserIdNotInDb, ProjectIdNotInDb));

        const string Auth1UserId = "auth0|cqc1q4rhv38htvclsst76lsr";
        Auth auth1 = new()
        {
            UserIds = [Auth1UserId],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            User = new()
            {
                DisplayName = "John Doe",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
            }
        };
        await applicationDbContext.AddAsync(auth1);
        await applicationDbContext.SaveChangesAsync();
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await projectRepository.AddCollaboratorAsync(auth1.User.Id, CollaboratorUserIdNotInDb, ProjectIdNotInDb));

        const string Auth2UserId = "auth0|44ovthw8qabskgsay3h5vxtd";
        Auth auth2 = new()
        {
            UserIds = [Auth2UserId],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            User = new()
            {
                DisplayName = "Jane Doe",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
            }
        };
        await applicationDbContext.AddAsync(auth2);
        await applicationDbContext.SaveChangesAsync();
        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await projectRepository.AddCollaboratorAsync(auth1.User.Id, auth2.User.Id, ProjectIdNotInDb));

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "This is a test.",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };
        testProject.Users.Add(auth1.User);

        ProjectPermission addCollaboratorPermission = await applicationDbContext.ProjectPermissions.FirstAsync(x => x.Type == BackendClassLib.ProjectPermissionType.AddCollaborator);
        testProject.UserProjectPermissions.Add(new()
        {
            User = auth1.User,
            Project = testProject,
            ProjectPermission = addCollaboratorPermission
        });
        await applicationDbContext.AddAsync(testProject);
        await applicationDbContext.SaveChangesAsync();

        const string Auth3UserId = "8uth9fz7yrfnrevlh3k1yq0x";
        Auth auth3 = new()
        {
            UserIds = [Auth3UserId],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            User = new()
            {
                DisplayName = "FireBranchDev",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
            }
        };
        await applicationDbContext.AddAsync(auth3);
        await applicationDbContext.SaveChangesAsync();
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await projectRepository.AddCollaboratorAsync(auth3.User.Id, auth2.User.Id, testProject.Id));

        await projectRepository.AddCollaboratorAsync(auth1.User.Id, auth2.User.Id, testProject.Id);
        testProject = await applicationDbContext.Projects.Include(x => x.Users).FirstAsync(c => c.Id == testProject.Id);
        Assert.Contains(auth2.User, testProject.Users);

        await Assert.ThrowsAsync<InsufficientPermissionToAddCollaboratorException>(async () => await projectRepository.AddCollaboratorAsync(auth2.User.Id, auth3.User.Id, testProject.Id));

        applicationDbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task RemoveCollaboratorAsync()
    {
        using ApplicationDbContext applicationDbContext = DatabaseFixture.CreateContext();
        await applicationDbContext.Database.BeginTransactionAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int UserIdNotInDb = 1000;
        const int CollaboratorToRemoveUserIdNotInDb = 1000;
        const int ProjectIdNotInDb = 1000;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await projectRepository.RemoveCollaboratorAsync(UserIdNotInDb, CollaboratorToRemoveUserIdNotInDb, ProjectIdNotInDb));

        Auth testUser1 = new()
        {
            UserIds = ["auth0|thpsr5x0ysmxuv1nm1yztd6z"],
            User = new()
            {
                DisplayName = "Test User 1"
            }
        };
        await applicationDbContext.AddAsync(testUser1);
        await applicationDbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await projectRepository.RemoveCollaboratorAsync(UserIdNotInDb, CollaboratorToRemoveUserIdNotInDb, ProjectIdNotInDb));

        Auth testUser2 = new()
        {
            UserIds = ["auth0|uyrlbivwkx5rm3rohmhx8u5d"],
            User = new()
            {
                DisplayName = "Test User 2"
            }
        };
        await applicationDbContext.AddAsync(testUser2);
        await applicationDbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await projectRepository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, ProjectIdNotInDb));

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "Test Project's description"
        };
        await applicationDbContext.AddAsync(testProject);
        await applicationDbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await projectRepository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id));

        testProject.Users.Add(testUser1.User);
        await applicationDbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await projectRepository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id));

        testProject.Users.Add(testUser2.User);
        await applicationDbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InsufficientPermissionToRemoveCollaboratorException>(async () => await projectRepository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id));

        int permissionId = await applicationDbContext.ProjectPermissions.Where(c => c.Type == BackendClassLib.ProjectPermissionType.RemoveCollaborator)
            .Select(c => c.Id).SingleAsync();

        await applicationDbContext.UserProjectPermissions.AddAsync(new()
        {
            UserId = testUser1.User.Id,
            ProjectId = testProject.Id,
            ProjectPermissionId = permissionId
        });
        await applicationDbContext.SaveChangesAsync();

        await projectRepository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id);
        bool isTestUser2Removed = await applicationDbContext.Projects.AnyAsync(c => c.Id == testProject.Id && c.Users.Any(c => c.Id == testUser2.User.Id));
        Assert.False(isTestUser2Removed);

        applicationDbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetAllProjectsAsync()
    {
        using ApplicationDbContext applicationDbContext = DatabaseFixture.CreateContext();
        await applicationDbContext.Database.BeginTransactionAsync();

        IProjectPermissionRepository projectPermissionRepository = new ProjectPermissionRepository(applicationDbContext);
        ProjectRepository projectRepository = new(applicationDbContext, projectPermissionRepository);

        const int UserIdNotInDb = 1000;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await projectRepository.GetAllProjectsAsync(UserIdNotInDb));

        Auth auth = new()
        {
            UserIds = ["auth0|thpsr5x0ysmxuv1nm1yztd6z"],
            User = new()
            {
                DisplayName = "Testing User 1"
            }
        };
        await applicationDbContext.AddAsync(auth);
        await applicationDbContext.SaveChangesAsync();

        Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        testProject1.Users.Add(auth.User);

        Project testProject2 = new()
        {
            Name = "Test Project 2"
        };
        testProject2.Users.Add(auth.User);

        Project testProject3 = new()
        {
            Name = "Test Project 3"
        };
        testProject3.Users.Add(auth.User);

        await applicationDbContext.AddRangeAsync([testProject1, testProject2, testProject3]);
        await applicationDbContext.SaveChangesAsync();

        List<Project> testingUser1Projects = await projectRepository.GetAllProjectsAsync(auth.User.Id);
        Assert.Equal(3, testingUser1Projects.Count);

        applicationDbContext.ChangeTracker.Clear();
    }
}

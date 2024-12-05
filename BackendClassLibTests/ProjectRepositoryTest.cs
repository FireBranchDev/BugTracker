namespace BackendClassLibTests;

public class ProjectRepositoryTest : BaseDbCollection
{
    [Fact]
    public async Task AddAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        ProjectRepository repository = new(context);

        const string Name = "Project One";
        const string Description = "Generic description";
        int authId = 1000;

        await Assert.ThrowsAsync<AuthNotFoundException>(async () => await repository.AddAsync(Name, Description, authId));

        const string UserId1 = "auth0|w5u554mylojeaqzufm36bpgv";
        const string UserId2 = "auth0|bmxbr8lmfwscoc7yeom5lqx6";
        Auth auth = new()
        {
            UserIds = [UserId1, UserId2],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        await context.AddAsync(auth);
        await context.SaveChangesAsync();
        authId = auth.Id;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.AddAsync(Name, Description, authId));

        User user = new()
        {
            DisplayName = "John Doe",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            AuthId = authId
        };
        await context.AddAsync(user);
        await context.SaveChangesAsync();

        int insertedProjectId = await repository.AddAsync(Name, Description, authId);
        bool isInsertedProjectSaved = await context.Projects.AnyAsync(c => c.Id == insertedProjectId);
        Assert.True(isInsertedProjectSaved);

        bool isUserInProject = await context.Projects.AnyAsync(c => c.Id == insertedProjectId && c.Users.Contains(user));
        Assert.True(isUserInProject);

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task AddCollaboratorAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        ProjectRepository repository = new(context);

        const int UserIdNotInDb = 1000;
        const int CollaboratorUserIdNotInDb = 1000;
        const int ProjectIdNotInDb = 1000;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.AddCollaboratorAsync(UserIdNotInDb, CollaboratorUserIdNotInDb, ProjectIdNotInDb));

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
        await context.AddAsync(auth1);
        await context.SaveChangesAsync();
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.AddCollaboratorAsync(auth1.User.Id, CollaboratorUserIdNotInDb, ProjectIdNotInDb));

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
        await context.AddAsync(auth2);
        await context.SaveChangesAsync();
        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await repository.AddCollaboratorAsync(auth1.User.Id, auth2.User.Id, ProjectIdNotInDb));

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "This is a test.",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };
        testProject.Users.Add(auth1.User);

        ProjectPermission addCollaboratorPermission = await context.ProjectPermissions.FirstAsync(x => x.Type == BackendClassLib.ProjectPermissionType.AddCollaborator);
        testProject.UserProjectPermissions.Add(new()
        {
            User = auth1.User,
            Project = testProject,
            ProjectPermission = addCollaboratorPermission
        });
        await context.AddAsync(testProject);
        await context.SaveChangesAsync();

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
        await context.AddAsync(auth3);
        await context.SaveChangesAsync();
        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await repository.AddCollaboratorAsync(auth3.User.Id, auth2.User.Id, testProject.Id));

        await repository.AddCollaboratorAsync(auth1.User.Id, auth2.User.Id, testProject.Id);
        testProject = await context.Projects.Include(x => x.Users).FirstAsync(c => c.Id == testProject.Id);
        Assert.Contains(auth2.User, testProject.Users);

        await Assert.ThrowsAsync<InsufficientPermissionToAddCollaboratorException>(async () => await repository.AddCollaboratorAsync(auth2.User.Id, auth3.User.Id, testProject.Id));

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task RemoveCollaboratorAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        ProjectRepository repository = new(context);

        const int UserIdNotInDb = 1000;
        const int CollaboratorToRemoveUserIdNotInDb = 1000;
        const int ProjectIdNotInDb = 1000;

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.RemoveCollaboratorAsync(UserIdNotInDb, CollaboratorToRemoveUserIdNotInDb, ProjectIdNotInDb));

        Auth testUser1 = new()
        {
            UserIds = ["auth0|thpsr5x0ysmxuv1nm1yztd6z"],
            User = new()
            {
                DisplayName = "Test User 1"
            }
        };
        await context.AddAsync(testUser1);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.RemoveCollaboratorAsync(UserIdNotInDb, CollaboratorToRemoveUserIdNotInDb, ProjectIdNotInDb));

        Auth testUser2 = new()
        {
            UserIds = ["auth0|uyrlbivwkx5rm3rohmhx8u5d"],
            User = new()
            {
                DisplayName = "Test User 2"
            }
        };
        await context.AddAsync(testUser2);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<ProjectNotFoundException>(async () => await repository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, ProjectIdNotInDb));

        Project testProject = new()
        {
            Name = "Test Project",
            Description = "Test Project's description"
        };
        await context.AddAsync(testProject);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await repository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id));

        testProject.Users.Add(testUser1.User);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotProjectCollaboratorException>(async () => await repository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id));

        testProject.Users.Add(testUser2.User);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InsufficientPermissionToRemoveCollaboratorException>(async () => await repository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id));

        int permissionId = await context.ProjectPermissions.Where(c => c.Type == BackendClassLib.ProjectPermissionType.RemoveCollaborator)
            .Select(c => c.Id).SingleAsync();

        await context.UserProjectPermissions.AddAsync(new()
        {
            UserId = testUser1.User.Id,
            ProjectId = testProject.Id,
            ProjectPermissionId = permissionId
        });
        await context.SaveChangesAsync();

        await repository.RemoveCollaboratorAsync(testUser1.User.Id, testUser2.User.Id, testProject.Id);
        bool isTestUser2Removed = await context.Projects.AnyAsync(c => c.Id == testProject.Id && c.Users.Any(c => c.Id == testUser2.User.Id));
        Assert.False(isTestUser2Removed);

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetAllProjectsAsync()
    {
        using ApplicationDbContext dbContext = DatabaseFixture.CreateContext();
        await dbContext.Database.BeginTransactionAsync();

        ProjectRepository projectRepository = new(dbContext);

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
        await dbContext.AddAsync(auth);
        await dbContext.SaveChangesAsync();

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

        await dbContext.AddRangeAsync([testProject1, testProject2, testProject3]);
        await dbContext.SaveChangesAsync();

        List<Project> testingUser1Projects = await projectRepository.GetAllProjectsAsync(auth.User.Id);
        Assert.Equal(3, testingUser1Projects.Count);

        dbContext.ChangeTracker.Clear();
    }
}

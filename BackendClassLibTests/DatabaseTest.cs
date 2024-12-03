namespace BackendClassLibTests;

public class DatabaseTest : BaseDbCollection
{
    [Fact]
    public async Task DoesProjectOwnerHaveAddCollaboratorPermission()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        const string Auth1UserId = "auth0|v785ly77bq46in5c7ufyd4p7";
        Auth auth1 = new()
        {
            UserIds = [Auth1UserId],
            User = new()
            {
                DisplayName = "John Doe"
            }
        };
        Project testProject = new()
        {
            Name = "Test",
            Description = "This a test project."
        };

        User projectOwner = auth1.User;
        ProjectPermission addCollaboratorPermission = await context.ProjectPermissions.FirstAsync(c => c.Type == BackendClassLib.ProjectPermissionType.AddCollaborator);
        testProject.UserProjectPermissions.Add(new()
        {
            User = projectOwner,
            Project = testProject,
            ProjectPermission = addCollaboratorPermission
        });
        auth1.User.Projects.Add(testProject);

        await context.AddAsync(auth1);
        await context.SaveChangesAsync();

        bool doesProjectOwnerHaveAddCollaboratorPermissions = await context.UserProjectPermissions.AnyAsync(c => c.User == auth1.User && c.Project == testProject && c.ProjectPermission.Type == BackendClassLib.ProjectPermissionType.AddCollaborator);
        Assert.True(doesProjectOwnerHaveAddCollaboratorPermissions);

        const string Auth2UserId = "auth0|h326a1cx1ok2re6zn341u2vi";
        Auth auth2 = new()
        {
            UserIds = [Auth2UserId],
            User = new()
            {
                DisplayName = "Jane Doe"
            }
        };
        auth2.User.Projects.Add(testProject);
        await context.AddAsync(auth2);
        await context.SaveChangesAsync();

        bool doesAddedUserHaveAddCollaboratorPermissions = await context.UserProjectPermissions.AnyAsync(c => c.User == auth2.User && c.Project == testProject && c.ProjectPermission.Type == BackendClassLib.ProjectPermissionType.AddCollaborator);
        Assert.False(doesAddedUserHaveAddCollaboratorPermissions);

        context.ChangeTracker.Clear();
    }
}

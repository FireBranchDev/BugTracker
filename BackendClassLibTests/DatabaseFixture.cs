using BackendClassLib;

namespace BackendClassLibTests;

public class DatabaseFixture : IDisposable
{
    const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=BugTrackerTest;Trusted_Connection=True;ConnectRetryCount=0";

    static readonly object _lock = new();
    static bool _databaseInitialized;

    public DatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    ProjectPermission addCollaborator = new()
                    {
                        Name = "Add Collaborator",
                        Description = "Allows a user to add a collaborator to the project.",
                        Type = ProjectPermissionType.AddCollaborator
                    };
                    ProjectPermission removeCollaborator = new()
                    {
                        Name = "Remove Collaborator",
                        Description = "Allows a user to remove a collaborator from the project.",
                        Type = ProjectPermissionType.RemoveCollaborator
                    };
                    context.AddRange([addCollaborator, removeCollaborator]);

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public static ApplicationDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);

    public void Dispose()
    {
        using var context = CreateContext();
        GC.SuppressFinalize(this);
    }
}

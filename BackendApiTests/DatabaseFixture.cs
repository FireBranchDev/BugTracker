using BackendClassLib.Database;
using Microsoft.EntityFrameworkCore;

namespace BackendApiTests;

public class DatabaseFixture : IDisposable
{
    public const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=BugTrackerTest;Trusted_Connection=True;ConnectRetryCount=0";

    static readonly object _lock = new();
    static bool _databaseInitialised;

    public DatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialised)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.SaveChanges();
                }

                _databaseInitialised = true;
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

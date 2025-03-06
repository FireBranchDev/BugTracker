using BackendClassLib.Database;

namespace BackendApi.Testing;

internal class DbInitializer
{
    internal static void Initialize(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        dbContext.Database.EnsureCreated();
        dbContext.SaveChanges();
    }
}

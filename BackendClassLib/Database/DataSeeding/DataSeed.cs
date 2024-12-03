namespace BackendClassLib.Database.DataSeeding;

public abstract class DataSeed(ApplicationDbContext db) : IDataSeed
{
    public ApplicationDbContext Db { get; } = db;

    public virtual async Task InitAsync()
    {
        await Db.Database.EnsureCreatedAsync();
    }
}

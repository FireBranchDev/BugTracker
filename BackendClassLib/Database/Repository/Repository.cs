namespace BackendClassLib.Database.Repository;

public abstract class Repository(ApplicationDbContext context)
{
    public ApplicationDbContext Context => context;
}

namespace BackendClassLib.Database.Repository;

public class Repository(ApplicationDbContext context) : IRepository
{
    public ApplicationDbContext Context => context;

    public IUnitOfWork UnitOfWork
    {
        get
        {
            return context;
        }
    }
}

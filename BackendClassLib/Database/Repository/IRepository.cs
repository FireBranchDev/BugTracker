namespace BackendClassLib.Database.Repository;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}

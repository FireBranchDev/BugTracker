using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public class BugPermissionUserRepository(ApplicationDbContext context) : IBugPermissionUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public IUnitOfWork UnitOfWork
    {
        get
        {
            return _context;
        }
    }

    public BugPermissionUser Add(BugPermissionUser bugPermissionUser)
    {
        _context.BugPermissionUsers.Add(bugPermissionUser);
        return bugPermissionUser;
    }
}

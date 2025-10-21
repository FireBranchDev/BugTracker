using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public class BugBugPermissionUserRepository(ApplicationDbContext context) : IBugBugPermissionUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public IUnitOfWork UnitOfWork
    {
        get
        {
            return _context;
        }
    }

    public BugBugPermissionUser Add(BugBugPermissionUser bugPermissionUser)
    {
        _context.BugBugPermissionUsers.Add(bugPermissionUser);
        return bugPermissionUser;
    }
}

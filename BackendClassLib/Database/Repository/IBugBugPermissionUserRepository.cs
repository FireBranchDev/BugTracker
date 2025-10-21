using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IBugBugPermissionUserRepository : IRepository
{
    BugBugPermissionUser Add(BugBugPermissionUser bugPermissionUser);
}

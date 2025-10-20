using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IBugPermissionUserRepository
{
    BugPermissionUser Add(BugPermissionUser bugPermissionUser);
}

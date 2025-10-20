using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IBugPermissionRepository
{
    public Task<List<BugPermission>> GetAllAsync();
}

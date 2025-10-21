using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IBugPermissionRepository : IRepository
{
    public Task<List<BugPermission>> GetAllAsync();
}

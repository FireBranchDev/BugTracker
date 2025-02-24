using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IProjectPermissionRepository
{
    public Task<List<ProjectPermission>> GetAllAsync();
}

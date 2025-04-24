using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IProjectPermissionRepository
{
    public Task<List<ProjectPermission>> GetAllAsync();
    public Task<bool> HasPermissionAsync(int projectId, int userId, ProjectPermissionType projectPermissionType);
}

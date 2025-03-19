using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IProjectDefaultRolesRepository
{
    Task<List<string>> GetAllRolesNamesAsync();
    Task<List<DefaultProjectRole>> GetAllRolesAsync();
    Task<List<ProjectPermission>> GetRolePermissionsAsync(int defaultRoleId);
}

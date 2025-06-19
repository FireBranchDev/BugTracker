namespace BackendClassLib.Database.Repository;

public interface IProjectRolesRepository
{
    Task<bool> IsOwnerAsync(int projectId, int userId);
}

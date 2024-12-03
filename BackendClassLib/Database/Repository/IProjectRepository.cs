namespace BackendClassLib.Database.Repository;

public interface IProjectRepository
{
    Task<int> AddAsync(string name, string? description, int authId);
    Task AddCollaboratorAsync(int userId, int userIdOfCollaboratorToAdd, int projectId);
    Task RemoveCollaboratorAsync(int userId, int userIdOfCollaboratorToRemove, int projectId);
}

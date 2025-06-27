using BackendClassLib.Database.Models;
using BackendClassLib.Models;

namespace BackendClassLib.Database.Repository;

public interface IProjectRepository
{
    Task<int> AddAsync(string name, string? description, int authId);
    Task AddCollaboratorAsync(int userId, int userIdOfCollaboratorToAdd, int projectId);
    Task RemoveCollaboratorAsync(int userId, int userIdOfCollaboratorToRemove, int projectId);
    Task<List<Project>> GetAllProjectsAsync(int userId);
    Task<Project> FindAsync(int projectId, int userId);
    Task<Project?> FindAsync(int projectId);
    Task DeleteAsync(int projectId, int userId);
    Task<bool> HasPermissionToPerformActionAsync(int projectId, int userId, ProjectPermissionType projectPermissionType);
    Task<List<Collaborator>> RetrieveCollaboratorsAsync(int projectId, int userId, byte take = 10, int? lastRetrievedUserId = null);
    Task<bool> AddCollaboratorsAsync(int projectId, int userId, List<int> userIdsToAdd);
}

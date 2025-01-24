using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IBugRepository
{
    Task CreateBugAsync(int projectId, int userId, string title, string? description);
    Task<List<Bug>> GetBugsAsync(int projectId, int userId);
    Task DeleteBugAsync(int projectId, int userId, int bugId);
    Task MarkBugAsAssigned(int bugId, int projectId, int userId);
    Task AssignCollaboratorToBugAsync(int bugId, int userId, int assigneeUserId);
    Task<List<User>> GetAssignedCollaborators(int bugId, int userId);
    Task UnassignCollaboratorAsync(int bugId, int userId, int assignedCollaboratorUserId);
}

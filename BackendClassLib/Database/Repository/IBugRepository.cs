using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IBugRepository
{
    Task CreateBugAsync(int projectId, int userId, string title, string? description);
    Task<List<Bug>> GetBugsAsync(int projectId, int userId);
}

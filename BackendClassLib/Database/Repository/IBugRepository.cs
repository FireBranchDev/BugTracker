namespace BackendClassLib.Database.Repository;

public interface IBugRepository
{
    Task CreateBugAsync(int projectId, int userId, string title, string? description);
}

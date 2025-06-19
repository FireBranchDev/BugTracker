using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IUserRepository
{
    Task AddAsync(string displayName, int authId);
    Task<User> FindAsync(int authId);
    Task UpdateDisplayNameAsync(int userId, string displayName);
    Task<List<User>> SearchByDisplayNameAsync(string displayName, byte limit, uint? lastSeenUserId);
    Task<List<BackendClassLib.Models.User>> SearchByDisplayNameAsync(string displayName, byte limit, uint? lastRetrievedUserId, int excludeFromProjectId);
    IAsyncEnumerable<User> Search(string? displayName, int? excludeFromProjectId, int limit = 10, int lastRetrievedId = 0);
}

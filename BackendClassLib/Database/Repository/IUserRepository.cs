using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IUserRepository
{
    Task AddAsync(string displayName, int authId);
    Task<User> FindAsync(int authId);
    Task UpdateDisplayNameAsync(int userId, string displayName);
}

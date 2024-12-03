using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.Repository;

public interface IAuthRepository
{
    Task<Auth> InsertAsync(string userId);
    Task LinkUserIdAsync(string userIdStored, string userIdToAdd);
    Task<Auth> FindAsync(string userId);
}

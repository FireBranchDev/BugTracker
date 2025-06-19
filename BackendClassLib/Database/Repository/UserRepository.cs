using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BackendClassLib.Database.Repository;

public class UserRepository(ApplicationDbContext context) : Repository(context), IUserRepository
{
    public async Task AddAsync(string displayName, int authId)
    {
        if (!await Context.Auths.AnyAsync(c => c.Id == authId)) throw new AuthNotFoundException();
        if (await Context.Users.AnyAsync(c => c.AuthId == authId)) throw new ExistingUserAccountException();
        User user = new()
        {
            DisplayName = displayName,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            AuthId = authId,
        };
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
    }

    public async Task<User> FindAsync(int authId)
    {
        if (!await Context.Auths.AnyAsync(c => c.Id == authId)) throw new AuthNotFoundException();
        if (!await Context.Users.AnyAsync(c => c.AuthId == authId)) throw new UserNotFoundException();
        return await Context.Users.Where(c => c.AuthId == authId).FirstAsync();
    }

    public IAsyncEnumerable<User> Search(string? displayName, int? excludeFromProjectId, int limit = 10, int lastRetrievedId = 0)
    {
        IQueryable<User> query = Context.Users.OrderBy(x => x.Id).Where(c => c.Id > lastRetrievedId);

        if (displayName != null)
        {
            query = query.Where(c => c.DisplayName.Contains(displayName));
        }

        if (excludeFromProjectId != null)
        {
            query = query.Where(c => !c.ProjectUsers.Any(c => c.ProjectId == excludeFromProjectId));
        }

        return query.Take(limit).AsAsyncEnumerable();
    }

    public async Task<List<User>> SearchByDisplayNameAsync(string displayName, byte limit = 10, uint? lastSeenUserId = null)
    {
        if (lastSeenUserId.HasValue)
        {
            return await Context.Users.Where(c => c.DisplayName.Contains(displayName) && c.Id > lastSeenUserId).Take(limit).OrderByDescending(c => c.Id).ToListAsync();
        }

        return await Context.Users.Where(c => c.DisplayName.Contains(displayName)).OrderByDescending(c => c.Id).ToListAsync();
    }

    public async Task<List<BackendClassLib.Models.User>> SearchByDisplayNameAsync(string displayName, byte limit, uint? lastRetrievedUserId, int excludeFromProjectId)
    {
        if (!await Context.Projects.AnyAsync(c => c.Id == excludeFromProjectId)) throw new ProjectNotFoundException();
        return await Context.Users.Where(u => u.DisplayName.Contains(displayName) && !u.Projects.Any(c => c.Id == excludeFromProjectId)).Take(limit).Order().Select(j => new
        {
            j.Id,
            j.DisplayName
        }).Select(x => new BackendClassLib.Models.User
        {
            Id = x.Id,
            DisplayName = x.DisplayName,
        }).ToListAsync();
    }

    public async Task UpdateDisplayNameAsync(int authId, string displayName)
    {
        Auth? auth = await Context.Auths.FindAsync(authId) ?? throw new AuthNotFoundException();
        User? user = await Context.Users.FirstOrDefaultAsync(c => c.AuthId == authId) ?? throw new UserNotFoundException();

        const int minimumDisplayNameLength = 3;
        const int maximumDisplayNameLength = 40;

        string pattern = $"^[^\\s_-]+(?:\\s+[^\\s_-]+)\\{minimumDisplayNameLength,maximumDisplayNameLength}*$";
        Match match = Regex.Match(displayName, pattern);
        if (!match.Success) throw new UserInvalidDisplayNameException();

        user.DisplayName = displayName;

        await Context.SaveChangesAsync();
    }
}

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

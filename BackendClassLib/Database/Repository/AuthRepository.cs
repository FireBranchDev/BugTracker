using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class AuthRepository(ApplicationDbContext context) : Repository(context), IAuthRepository
{
    public async Task<Auth> InsertAsync(string userId)
    {
        if (await Context.Auths.AnyAsync(c => c.UserIds.Contains(userId))) throw new UsedAuthUserIdException();
        Auth newAuth = new()
        {
            UserIds = [userId]
        };
        await Context.Auths.AddAsync(newAuth);
        await Context.SaveChangesAsync();

        return newAuth;
    }

    public async Task LinkUserIdAsync(string userId, string userIdToLink)
    {
        if (!await Context.Auths.AnyAsync(c => c.UserIds.Contains(userId))) throw new AuthUserIdNotFoundException();
        if (await Context.Auths.AnyAsync(c => c.UserIds.Contains(userIdToLink))) throw new UsedAuthUserIdException();

        Auth? auth = await Context.Auths.Where(c => c.UserIds.Contains(userId)).FirstOrDefaultAsync() ?? throw new AuthNotFoundException();
        auth.UserIds.Add(userIdToLink);

        await Context.SaveChangesAsync();
    }

    public async Task<Auth> FindAsync(string userId)
    {
        Auth? auth = await Context.Auths.Where(c => c.UserIds.Contains(userId)).FirstOrDefaultAsync() ?? throw new AuthUserIdNotFoundException();
        return auth;
    }
}

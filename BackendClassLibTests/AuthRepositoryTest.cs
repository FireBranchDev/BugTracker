namespace BackendClassLibTests;

[Collection(nameof(DatabaseCollection))]
public class AuthRepositoryTest
{
    [Fact]
    public async Task InsertAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        context.Database.BeginTransaction();

        AuthRepository repository = new(context);

        string userId = "auth0|aldpbv65op70oof1a5328yiy";
        await repository.InsertAsync(userId);

        Auth savedAuth = await context.Auths.Where(c => c.UserIds.Contains(userId)).FirstAsync();
        Assert.Contains(userId, savedAuth.UserIds);

        await Assert.ThrowsAsync<UsedAuthUserIdException>(async () => await repository.InsertAsync(userId));

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task LinkUserIdAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        context.Database.BeginTransaction();

        AuthRepository repository = new(context);

        string userIdNotStoredInDb = "auth0|j39p466uax87xr6l0qfugjaz";
        await Assert.ThrowsAsync<AuthUserIdNotFoundException>(async () => await repository.LinkUserIdAsync(userIdNotStoredInDb, "auth0|ocafnou9rxzofcsyumum5dqn"));

        string auth1UserId = "auth0|t6ud1munj9o7whz56o2wi81q";
        Auth auth1 = new()
        {
            UserIds = [auth1UserId],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };

        string auth2UserId = "auth0|qki5rkfcqc2rwa9t3n4ln8je";
        Auth auth2 = new()
        {
            UserIds = [auth2UserId],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };

        await context.Auths.AddRangeAsync([auth1, auth2]);
        await context.SaveChangesAsync();

        string newUserIdToLinkAuth1 = "auth0|vt36vdhrvh85mm805xq0zfmi";
        await repository.LinkUserIdAsync(auth1UserId, newUserIdToLinkAuth1);
        Auth? linkedUserId = await context.Auths.FirstOrDefaultAsync(c => c.UserIds.Contains(auth1UserId) && c.UserIds.Contains(newUserIdToLinkAuth1));
        Assert.NotNull(linkedUserId);

        await Assert.ThrowsAsync<UsedAuthUserIdException>(async () => await repository.LinkUserIdAsync(auth1UserId, auth2UserId));

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task FindAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        context.Database.BeginTransaction();

        AuthRepository repository = new(context);

        string userIdNotStored = "auth0|j39p466uax87xr6l0qfugjaz";
        await Assert.ThrowsAsync<AuthUserIdNotFoundException>(async () => await repository.FindAsync(userIdNotStored));

        string firstUserIdAuth1 = "auth0|3imv1lg68shtrard6cdn3mq6";
        string secondUserIdAuth1 = "auth0|bfgprzn6j9jb4b9dk2l0wdm8";
        Auth auth1 = new()
        {
            UserIds = [firstUserIdAuth1, secondUserIdAuth1],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };

        await context.Auths.AddAsync(auth1);
        await context.SaveChangesAsync();

        Assert.Equal(auth1, await repository.FindAsync(firstUserIdAuth1));
        Assert.Equal(auth1, await repository.FindAsync(secondUserIdAuth1));

        context.ChangeTracker.Clear();
    }
}

namespace BackendClassLibTests;

[Collection(nameof(DatabaseCollection))]
public class UserRepositoryTest
{
    [Fact]
    public async Task AddAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        Auth auth = new()
        {
            UserIds = ["auth0|uxj5m10n787zww5odkwin6hp", "auth0|de25egz7jd4fyl4t8y46f27v"],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            User = new()
            {
                DisplayName = "John Doe",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            }
        };

        await context.AddAsync(auth);
        await context.SaveChangesAsync();

        UserRepository repository = new(context);
        await Assert.ThrowsAsync<AuthNotFoundException>(async () => await repository.AddAsync("John Doe", 1000));
        await Assert.ThrowsAsync<ExistingUserAccountException>(async () => await repository.AddAsync("John Doe", auth.Id));

        auth.User = null;
        await context.SaveChangesAsync();

        await repository.AddAsync("John Doe", auth.Id);
        Assert.NotNull(auth.User);

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task FindAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        Auth auth = new()
        {
            UserIds = ["auth0|uxj5m10n787zww5odkwin6hp", "auth0|de25egz7jd4fyl4t8y46f27v"],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        await context.AddAsync(auth);
        await context.SaveChangesAsync();

        UserRepository repository = new(context);
        const int AuthIdMissingDb = 1000;
        await Assert.ThrowsAsync<AuthNotFoundException>(async () => await repository.FindAsync(AuthIdMissingDb));
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.FindAsync(auth.Id));

        auth.User = new()
        {
            DisplayName = "John Doe",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        await context.SaveChangesAsync();

        await repository.FindAsync(auth.Id);
        Assert.NotNull(auth.User);

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task UpdateDisplayNameAsync()
    {
        using ApplicationDbContext context = DatabaseFixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        UserRepository repository = new(context);

        const int AuthIdMissingDb = 1000;
        const string NewDisplayName = "LovesCode";

        await Assert.ThrowsAsync<AuthNotFoundException>(async () => await repository.UpdateDisplayNameAsync(AuthIdMissingDb, NewDisplayName));

        Auth auth = new()
        {
            UserIds = ["auth0|uxj5m10n787zww5odkwin6hp", "auth0|de25egz7jd4fyl4t8y46f27v"],
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        await context.AddAsync(auth);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayName));

        auth.User = new()
        {
            DisplayName = "John Doe",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
        await context.SaveChangesAsync();

        const string NewDisplayNameLessThanMinimum = "ab";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameLessThanMinimum));

        const string NewDisplayNameGreaterThanMaximum = "sDzoSXo1l47PzEZhyYa8d7SwJcIgSKcHa0FAjZg3UAQj";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameGreaterThanMaximum));

        const string NewDisplayNameWithUnderscores = "__________";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameWithUnderscores));

        const string NewDisplayNameWithUnderscoresLessThanMinimum = "__";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameWithUnderscoresLessThanMinimum));

        const string NewDisplayNameWithUnderscoresGreaterThanMaximum = "_____________________________________________________________________________________________________________";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameWithUnderscoresGreaterThanMaximum));

        const string NewDisplayNameWithHypens = "------------";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameWithHypens));

        const string NewDisplayNameWithHypensLessThanMinimum = "--";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameWithHypensLessThanMinimum));

        const string NewDisplayNameWithHypensGreaterThanMaximum = "---------------------------------------------------------------------------------------------------------------------";
        await Assert.ThrowsAsync<UserInvalidDisplayNameException>(async () => await repository.UpdateDisplayNameAsync(auth.Id, NewDisplayNameWithHypensGreaterThanMaximum));

        context.ChangeTracker.Clear();
    }
}

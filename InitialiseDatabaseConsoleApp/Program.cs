// See https://aka.ms/new-console-template for more information
using BackendClassLib;
using BackendClassLib.Database;
using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// Build a config object, using env vars and JSON providers.
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

DbContextOptions contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(config.GetConnectionString("Database"))
    .Options;

using ApplicationDbContext context = new(contextOptions);

DefaultProjectRole? owner = await context.DefaultProjectRoles.Where(x => x.Name == "Owner").FirstOrDefaultAsync();
if (owner is null)
{
    owner = new()
    {
        Name = "Owner",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    await context.AddAsync(owner);

    foreach (ProjectPermission permission in context.ProjectPermissions.ToList())
        owner.ProjectPermissions.Add(permission);

    await context.SaveChangesAsync();
}

DefaultProjectRole? collaborator = await context.DefaultProjectRoles.Where(x => x.Name == "Collaborator").FirstOrDefaultAsync();
if (collaborator is null)
{
    collaborator = new()
    {
        Name = "Collaborator",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    await context.AddAsync(collaborator);

    ProjectPermission? createBug = await context.ProjectPermissions.Where(x => x.Type == ProjectPermissionType.CreateBug).FirstOrDefaultAsync();
    if (createBug is not null)
        collaborator.ProjectPermissions.Add(createBug);

    await context.SaveChangesAsync();
}

// Generate testing users
Random rnd = new();

List<char> lowerCaseAlphabet = [];
for (char letter = 'a'; letter <= 'z'; letter++)
{
    lowerCaseAlphabet.Add(letter);
}

char RandomLowerCaseLetter()
{
    int count = lowerCaseAlphabet.Count;
    int num = rnd.Next(count);
    return lowerCaseAlphabet[num];
}

List<char> upperCaseAlphabet = [];
for (char letter = 'A'; letter <= 'Z'; letter++)
{
    upperCaseAlphabet.Add(letter);
}

char RandomUpperCaseLetter()
{
    int count = upperCaseAlphabet.Count;
    int num = rnd.Next(count);
    return upperCaseAlphabet[num];
}

List<byte> digits = [];
for (byte i = 0; i <= 9; i++)
{
    digits.Add(i);
}

byte RandomDigit()
{
    int count = digits.Count;
    int num = rnd.Next(count);
    return digits[num];
}

List<string> testingUsersAuthIds = [];

const int MaximumAuthIdLength = 24;

string RandomAuthId()
{
    string authId = "";
    for (int j = 1; j <= MaximumAuthIdLength; j++)
    {
        int num = rnd.Next(1, 4);
        switch (num)
        {
            case 1:
                authId += RandomLowerCaseLetter();
                break;
            case 2:
                authId += RandomUpperCaseLetter();
                break;
            case 3:
                authId += RandomDigit();
                break;
        }
    }

    return authId;
}

const int NumberOfTestingUsers = 20;
for (int i = 1; i <= NumberOfTestingUsers; i++)
{
    string randomAuthId = RandomAuthId();
    while (await context.Auths.Where(c => c.UserIds.Contains(randomAuthId)).AnyAsync())
    {
        randomAuthId = RandomAuthId();
    }

    Auth auth = new()
    {
        CreatedOn = DateTime.UtcNow,
        UpdatedOn = DateTime.UtcNow,
    };

    auth.UserIds.Add(randomAuthId);

    await context.Auths.AddAsync(auth);
    await context.SaveChangesAsync();

    User user = new()
    {
        DisplayName = $"Testing User {auth.Id}",
        CreatedOn = DateTime.UtcNow,
        UpdatedOn = DateTime.UtcNow,
    };

    auth.User = user;
}

await context.SaveChangesAsync();

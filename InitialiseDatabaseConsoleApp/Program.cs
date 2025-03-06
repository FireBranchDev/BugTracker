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

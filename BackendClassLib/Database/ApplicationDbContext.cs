using BackendClassLib.Database.Models;
using BackendClassLib.Database.TypeConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BackendClassLib.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Auth> Auths { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Project> Projects { get; set; } = null!;
    public virtual DbSet<ProjectPermission> ProjectPermissions { get; set; } = null!;
    public virtual DbSet<UserProjectPermission> UserProjectPermissions { get; set; } = null!;
    public virtual DbSet<Bug> Bugs { get; set; } = null!;
    public virtual DbSet<BugPermission> BugPermissions { get; set; } = null!;

    public virtual DbSet<BugPermissionUser> BugPermissionUsers { get; set; } = null!;

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AuthEntityTypeConfiguration().Configure(modelBuilder.Entity<Auth>());
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
        new ProjectEntityTypeConfiguration().Configure(modelBuilder.Entity<Project>());
        new BugEntityTypeConfiguration().Configure(modelBuilder.Entity<Bug>());
        new ProjectPermissionEntityTypeConfiguration().Configure(modelBuilder.Entity<ProjectPermission>());
        new UserProjectPermissionEntityTypeConfiguration().Configure(modelBuilder.Entity<UserProjectPermission>());

        modelBuilder.Entity<Bug>()
            .HasMany(x => x.BugPermissions)
            .WithMany(x => x.Bugs)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<BugPermission>()
            .HasMany(x => x.Bugs)
            .WithMany(x => x.BugPermissions)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<BugPermission>()
            .HasMany(x => x.Users)
            .WithMany(x => x.BugPermissions)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<User>()
            .HasMany(x => x.BugPermissions)
            .WithMany(x => x.Users)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<Bug>()
            .HasMany(x => x.AssignedUsers)
            .WithMany(x => x.AssignedBugs)
            .UsingEntity<BugAssignee>()
            .ToTable("BugAssignees");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSeeding((context, _) =>
            {
                if (!context.Set<BugPermission>().Where(c => c.Type == BugPermissionType.UpdateStatus).Any())
                {
                    context.Set<BugPermission>().Add(new BugPermission
                    {
                        Type = BugPermissionType.UpdateStatus,
                        CreatedAt = DateTime.UtcNow,
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.AddCollaborator).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Add Collaborator",
                        Description = "A project's permission to allow for adding a collaborator.",
                        Type = ProjectPermissionType.AddCollaborator,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.RemoveCollaborator).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Remove Collaborator",
                        Description = "A project's permission to allow for removing a collaborator.",
                        Type = ProjectPermissionType.RemoveCollaborator,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.DeleteBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Delete Bug",
                        Description = "A project's permission to allow for deleting a bug.",
                        Type = ProjectPermissionType.DeleteBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.AssignCollaboratorToBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Assign Collaborator to Bug",
                        Description = "A project's permission to allow for assigning a collaborator to a bug.",
                        Type = ProjectPermissionType.AssignCollaboratorToBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.UnassignCollaboratorFromBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Unassign Collaborator from Bug",
                        Description = "A project's permission to allow for unassigning a collaborator from a bug.",
                        Type = ProjectPermissionType.UnassignCollaboratorFromBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.DeleteProject).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Delete Project",
                        Description = "A project's permission to allow for deleting a project.",
                        Type = ProjectPermissionType.DeleteProject,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                context.SaveChanges();
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (!await context.Set<BugPermission>().Where(c => c.Type == BugPermissionType.UpdateStatus).AnyAsync(cancellationToken))
                {
                    await context.Set<BugPermission>().AddAsync(new()
                    {
                        Type = BugPermissionType.UpdateStatus,
                        CreatedAt = DateTime.UtcNow,
                    }, cancellationToken);
                }

                if (!await context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.AddCollaborator).AnyAsync(cancellationToken))
                {
                    await context.Set<ProjectPermission>().AddAsync(new()
                    {
                        Name = "Add Collaborator",
                        Description = "A project's permission to allow for adding a collaborator.",
                        Type = ProjectPermissionType.AddCollaborator,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    }, cancellationToken);
                }

                if (!await context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.RemoveCollaborator).AnyAsync(cancellationToken))
                {
                    await context.Set<ProjectPermission>().AddAsync(new()
                    {
                        Name = "Remove Collaborator",
                        Description = "A project's permission to allow for removing a collaborator.",
                        Type = ProjectPermissionType.RemoveCollaborator,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    }, cancellationToken);
                }

                if (!await context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.DeleteBug).AnyAsync(cancellationToken))
                {
                    await context.Set<ProjectPermission>().AddAsync(new ProjectPermission
                    {
                        Name = "Delete Bug",
                        Description = "A project's permission to allow for deleting a bug.",
                        Type = ProjectPermissionType.DeleteBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    }, cancellationToken);
                }

                if (!await context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.AssignCollaboratorToBug).AnyAsync(cancellationToken))
                {
                    await context.Set<ProjectPermission>().AddAsync(new ProjectPermission
                    {
                        Name = "Assign Collaborator to Bug",
                        Description = "A project's permission to allow for assigning a collaborator to a bug.",
                        Type = ProjectPermissionType.AssignCollaboratorToBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    }, cancellationToken);
                }

                if (!await context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.UnassignCollaboratorFromBug).AnyAsync(cancellationToken))
                {
                    await context.Set<ProjectPermission>().AddAsync(new ProjectPermission
                    {
                        Name = "Unassign Collaborator from Bug",
                        Description = "A project's permission to allow for unassigning a collaborator from a bug.",
                        Type = ProjectPermissionType.UnassignCollaboratorFromBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    }, cancellationToken);
                }

                if (!await context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.DeleteProject).AnyAsync(cancellationToken))
                {
                    await context.Set<ProjectPermission>().AddAsync(new ProjectPermission
                    {
                        Name = "Delete Project",
                        Description = "A project's permission to allow for deleting a project.",
                        Type = ProjectPermissionType.DeleteProject,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    }, cancellationToken);
                }
            });
    }
}

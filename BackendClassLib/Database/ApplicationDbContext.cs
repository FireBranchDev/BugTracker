using BackendClassLib.Database.Converters;
using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<Auth> Auths { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Project> Projects { get; set; } = null!;
    public virtual DbSet<ProjectPermission> ProjectPermissions { get; set; } = null!;
    public virtual DbSet<UserProjectPermission> UserProjectPermissions { get; set; } = null!;
    public virtual DbSet<Bug> Bugs { get; set; } = null!;
    public virtual DbSet<BugPermission> BugPermissions { get; set; } = null!;
    public virtual DbSet<BugPermissionUser> BugPermissionUsers { get; set; } = null!;
    public DbSet<DefaultProjectRole> DefaultProjectRoles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DateTimeUtcConverter dateTimeUtcConverter = new();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeUtcConverter);
                }
            }
        }

        modelBuilder.Entity<Base>().UseTpcMappingStrategy();

        modelBuilder.Entity<Project>()
            .HasMany(c => c.Users)
            .WithMany(c => c.Projects)
            .UsingEntity<ProjectUser>();

        modelBuilder.Entity<Bug>()
            .HasMany(b => b.Users)
            .WithMany(u => u.Bugs)
            .UsingEntity<BugUser>();

        modelBuilder.Entity<Bug>()
            .HasMany(c => c.BugPermissions)
            .WithMany(c => c.Bugs)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<User>()
            .HasMany(c => c.BugPermissions)
            .WithMany(c => c.Users)
            .UsingEntity<BugPermissionUser>();
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
                        Type = BugPermissionType.UpdateStatus
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.AddCollaborator).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Add Collaborator",
                        Description = "A project's permission to allow for adding a collaborator.",
                        Type = ProjectPermissionType.AddCollaborator
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.RemoveCollaborator).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Remove Collaborator",
                        Description = "A project's permission to allow for removing a collaborator.",
                        Type = ProjectPermissionType.RemoveCollaborator
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.DeleteBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Delete Bug",
                        Description = "A project's permission to allow for deleting a bug.",
                        Type = ProjectPermissionType.DeleteBug
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.AssignCollaboratorToBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Assign Collaborator to Bug",
                        Description = "A project's permission to allow for assigning a collaborator to a bug.",
                        Type = ProjectPermissionType.AssignCollaboratorToBug
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.UnassignCollaboratorFromBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Unassign Collaborator from Bug",
                        Description = "A project's permission to allow for unassigning a collaborator from a bug.",
                        Type = ProjectPermissionType.UnassignCollaboratorFromBug
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.DeleteProject).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Delete Project",
                        Description = "A project's permission to allow for deleting a project.",
                        Type = ProjectPermissionType.DeleteProject
                    });
                }

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.CreateBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Create Bug",
                        Description = "A project's permission to allow for creating a bug.",
                        Type = ProjectPermissionType.CreateBug
                    });
                }

                context.SaveChanges();
            });
    }

    void ManageBaseEntityDateTimeFields()
    {
        var entries = ChangeTracker
           .Entries()
           .Where(e => e.Entity is Base && (
                   e.State == EntityState.Added
                   || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((Base)entityEntry.Entity).Updated = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((Base)entityEntry.Entity).Created = DateTime.Now;
            }
        }
    }


    public override int SaveChanges()
    {
        ManageBaseEntityDateTimeFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ManageBaseEntityDateTimeFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}

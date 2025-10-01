using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
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
        modelBuilder.Entity<Project>()
            .HasMany(c => c.Users)
            .WithMany(c => c.Projects)
            .UsingEntity<ProjectUser>(j => j.Property(e => e.Joined).HasDefaultValueSql("GETUTCDATE()"));

        modelBuilder.Entity<Bug>()
            .HasMany(c => c.AssignedUsers)
            .WithMany(c => c.AssignedBugs)
            .UsingEntity<BugAssignee>();

        modelBuilder.Entity<Bug>()
            .HasMany(c => c.BugPermissions)
            .WithMany(c => c.Bugs)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<User>()
            .HasMany(c => c.BugPermissions)
            .WithMany(c => c.Users)
            .UsingEntity<BugPermissionUser>();

        modelBuilder.Entity<Bug>()
            .Property(b => b.CreatedOn)
            .HasDefaultValueSql("GETUTCDATE()");
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

                if (!context.Set<ProjectPermission>().Where(c => c.Type == ProjectPermissionType.CreateBug).Any())
                {
                    context.Set<ProjectPermission>().Add(new ProjectPermission
                    {
                        Name = "Create Bug",
                        Description = "A project's permission to allow for creating a bug.",
                        Type = ProjectPermissionType.CreateBug,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                    });
                }

                context.SaveChanges();
            });
    }
}

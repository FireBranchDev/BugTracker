using BackendClassLib.Database.Models;
using BackendClassLib.Database.TypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Auth> Auths { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectPermission> ProjectPermissions { get; set; } = null!;
    public DbSet<UserProjectPermission> UserProjectPermissions { get; set; } = null!;

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
    }
}

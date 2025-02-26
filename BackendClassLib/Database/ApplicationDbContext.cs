﻿using BackendClassLib.Database.Models;
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
                    context.SaveChanges();
                }
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
                    await context.SaveChangesAsync(cancellationToken);
                }
            });
    }
}

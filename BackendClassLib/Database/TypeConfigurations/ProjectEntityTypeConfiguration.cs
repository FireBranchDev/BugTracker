using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public class ProjectEntityTypeConfiguration : BaseEntityTypeConfiguration<Project>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.HasMany(x => x.Users)
            .WithMany(x => x.Projects);

        builder.HasMany(x => x.Bugs)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId);

        builder.HasMany(x => x.UserProjectPermissions)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId);
    }
}

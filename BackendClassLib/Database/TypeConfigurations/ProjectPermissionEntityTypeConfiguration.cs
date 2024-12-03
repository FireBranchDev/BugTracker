using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public class ProjectPermissionEntityTypeConfiguration : BaseEntityTypeConfiguration<ProjectPermission>
{
    public override void Configure(EntityTypeBuilder<ProjectPermission> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.HasIndex(x => x.Type)
            .IsUnique();

        builder.HasMany(x => x.UserProjectPermissions)
            .WithOne(x => x.ProjectPermission)
            .HasForeignKey(x => x.ProjectPermissionId);
    }
}

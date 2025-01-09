using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public class UserProjectPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserProjectPermission>
{
    public void Configure(EntityTypeBuilder<UserProjectPermission> builder)
    {
        builder.HasKey(c => new { c.UserId, c.ProjectId, c.ProjectPermissionId });

        builder.Property(c => c.CreatedOn)
            .IsRequired();

        builder.Property(c => c.UpdatedOn)
            .IsRequired();
    }
}

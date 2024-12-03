using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public class UserEntityTypeConfiguration : BaseEntityTypeConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.DisplayName)
            .HasMaxLength(40)
            .IsRequired();

        builder.HasMany(c => c.UserProjectPermissions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
    }
}

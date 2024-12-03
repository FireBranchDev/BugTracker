using BackendClassLib.Database.Models;

namespace BackendClassLib.Database.TypeConfigurations;

public class AuthEntityTypeConfiguration : BaseEntityTypeConfiguration<Models.Auth>
{
    public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Models.Auth> builder)
    {
        base.Configure(builder);

        builder.PrimitiveCollection(c => c.UserIds);

        builder.HasOne(c => c.User)
            .WithOne(c => c.Auth)
            .HasForeignKey<User>(c => c.AuthId)
            .IsRequired();
    }
}

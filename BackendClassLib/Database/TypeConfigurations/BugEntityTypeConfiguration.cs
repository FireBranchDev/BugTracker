using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public class BugEntityTypeConfiguration : BaseEntityTypeConfiguration<Bug>
{
    public override void Configure(EntityTypeBuilder<Bug> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title);
        builder.Property(x => x.Description);
    }
}

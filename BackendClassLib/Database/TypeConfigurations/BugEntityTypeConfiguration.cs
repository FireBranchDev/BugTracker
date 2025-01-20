using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public class BugEntityTypeConfiguration : BaseEntityTypeConfiguration<Bug>
{
    public override void Configure(EntityTypeBuilder<Bug> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasMany(x => x.AssignedUsers)
            .WithMany(x => x.AssignedBugs)
            .UsingEntity<BugAssignee>("BugAssignees",
                l => l.HasOne(e => e.User).WithMany(e => e.BugAssignees),
                r => r.HasOne(e => e.Bug).WithMany(e => e.BugAssignees));
    }
}

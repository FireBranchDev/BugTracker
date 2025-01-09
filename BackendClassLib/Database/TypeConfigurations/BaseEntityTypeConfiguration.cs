using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendClassLib.Database.TypeConfigurations;

public abstract class BaseEntityTypeConfiguration<TEntityType> : IEntityTypeConfiguration<TEntityType> where TEntityType : Base
{
    public virtual void Configure(EntityTypeBuilder<TEntityType> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CreatedOn)
            .IsRequired();

        builder.Property(c => c.UpdatedOn)
            .IsRequired();
    }
}

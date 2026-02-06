using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ModuleEntityConfiguration : BaseEntityConfiguration<ModuleEntity>
    {
        public override void Configure(EntityTypeBuilder<ModuleEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("MODULES");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .HasColumnName("PK_MODULE");

            builder.Property(m => m.ModuleName)
                .HasColumnName("MODULE_NAME")
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}

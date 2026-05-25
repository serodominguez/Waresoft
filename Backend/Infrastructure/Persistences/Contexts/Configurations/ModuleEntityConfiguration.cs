using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ModuleEntityConfiguration : BaseAuditEntityConfiguration<ModuleEntity>
    {
        public override void Configure(EntityTypeBuilder<ModuleEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("MODULES");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .HasColumnName("IdModule");

            builder.Property(m => m.ModuleName)
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}

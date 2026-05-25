using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class BrandEntityConfiguration : BaseAuditEntityConfiguration<BrandEntity>
    {
        public override void Configure(EntityTypeBuilder<BrandEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("BRANDS");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("IdBrand");

            builder.Property(b => b.BrandName)
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}

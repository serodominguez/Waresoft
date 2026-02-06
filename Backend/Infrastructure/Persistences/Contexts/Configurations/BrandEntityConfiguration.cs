using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class BrandEntityConfiguration : BaseEntityConfiguration<BrandEntity>
    {
        public override void Configure(EntityTypeBuilder<BrandEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("BRANDS");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("PK_BRAND");

            builder.Property(b => b.BrandName)
                .HasColumnName("BRAND_NAME")
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}

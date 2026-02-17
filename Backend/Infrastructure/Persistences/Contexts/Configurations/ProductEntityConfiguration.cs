using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ProductEntityConfiguration : BaseEntityConfiguration<ProductEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("PRODUCTS");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("PK_PRODUCT");

            builder.Property(p => p.Code)
                .HasColumnName("CODE")
                .HasMaxLength(25);

            builder.Property(p => p.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Material)
                .HasColumnName("MATERIAL")
                .HasMaxLength(25);

            builder.Property(p => p.Color)
                .HasColumnName("COLOR")
                .HasMaxLength(20);

            builder.Property(p => p.UnitMeasure)
                .HasColumnName("UNIT_MEASURE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(p => p.IdBrand)
                .HasColumnName("PK_BRAND")
                .IsRequired();

            builder.Property(p => p.IdCategory)
                .HasColumnName("PK_CATEGORY")
                .IsRequired();

            builder.Property(p => p.Replenishment)
                .HasColumnName("REPLENISHMENT");

            builder.HasOne(b => b.Brand)
                .WithMany(p => p.Product)
                .HasForeignKey(b => b.IdBrand);

            builder.HasOne(c => c.Category)
                .WithMany(p => p.Product)
                .HasForeignKey(c => c.IdCategory);
        }
    }
}

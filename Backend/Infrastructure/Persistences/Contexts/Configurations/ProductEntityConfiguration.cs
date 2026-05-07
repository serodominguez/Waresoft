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
                .HasColumnName("IdProduct");

            builder.Property(p => p.Code)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Material)
                .HasMaxLength(25);

            builder.Property(p => p.Color)
                .HasMaxLength(20);

            builder.Property(p => p.UnitMeasure)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(p => p.Image)
                .HasMaxLength(2048);

            builder.Property(p => p.IdBrand)
                .IsRequired();

            builder.Property(p => p.IdCategory)
                .IsRequired();

            builder.Property(p => p.Replenishment);

            builder.HasOne(b => b.Brand)
                .WithMany(p => p.Product)
                .HasForeignKey(b => b.IdBrand);

            builder.HasOne(c => c.Category)
                .WithMany(p => p.Product)
                .HasForeignKey(c => c.IdCategory);
        }
    }
}

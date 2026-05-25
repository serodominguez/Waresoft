using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsReceiptDetailsEntityConfiguration : IEntityTypeConfiguration<GoodsReceiptDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsReceiptDetailsEntity> builder)
        {
            builder.ToTable("GOODS_RECEIPT_DETAILS")
                .HasKey(d => new { d.IdReceipt, d.Item });

            builder.Property(d => d.IdReceipt)
                .IsRequired();

            builder.Property(d => d.Item)
                .IsRequired();

            builder.Property(d => d.IdProduct)
                .IsRequired();

            builder.Property(d => d.Quantity)
                .IsRequired();

            builder.Property(d => d.UnitCost)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.TotalCost)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.Item)
                .IsRequired();

            builder.HasOne(r => r.GoodsReceipt)
                .WithMany(d => d.GoodsReceiptDetails)
                .HasForeignKey(r => r.IdReceipt)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(d => d.GoodsReceiptDetails)
                .HasForeignKey(p => p.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

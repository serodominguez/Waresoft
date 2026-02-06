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
                .HasColumnName("PK_RECEIPT")
                .IsRequired();

            builder.Property(d => d.IdProduct)
                .HasColumnName("PK_PRODUCT")
                .IsRequired();

            builder.Property(d => d.Quantity)
                .HasColumnName("QUANTITY")
                .IsRequired();

            builder.Property(d => d.UnitCost)
                .HasColumnName("UNIT_COST")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.TotalCost)
                .HasColumnName("TOTAL_COST")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.Item)
                .HasColumnName("ITEM")
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

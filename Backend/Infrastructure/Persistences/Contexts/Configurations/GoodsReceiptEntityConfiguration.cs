using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsReceiptEntityConfiguration : BaseEntityConfiguration<GoodsReceiptEntity>
    {
        public override void Configure(EntityTypeBuilder<GoodsReceiptEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("GOODS_RECEIPT");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasColumnName("IdReceipt");

            builder.Property(r => r.Code)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(r => r.Type)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(r => r.DocumentDate);

            builder.Property(r => r.DocumentType)
                .HasMaxLength(15);

            builder.Property(r => r.DocumentNumber)
                .HasMaxLength(30);

            builder.Property(r => r.TotalAmount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(r => r.Annotations)
                .HasMaxLength(80);

            builder.Property(r => r.IdSupplier);

            builder.Property(r => r.IdStore)
                .IsRequired();

            builder.Property(r => r.Status);

            builder.HasOne(s => s.Supplier)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(s => s.IdSupplier);

            builder.HasOne(s => s.Store)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

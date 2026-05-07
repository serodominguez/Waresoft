using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsReceiptEntityConfiguration : IEntityTypeConfiguration<GoodsReceiptEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsReceiptEntity> builder)
        {
            builder.ToTable("GOODS_RECEIPT");

            builder.HasKey(r => r.IdReceipt);
            builder.Property(r => r.IdReceipt);

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

            builder.Property(r => r.AuditCreateUser);

            builder.Property(r => r.AuditCreateDate);

            builder.Property(r => r.AuditDeleteUser);

            builder.Property(r => r.AuditDeleteDate);

            builder.Property(r => r.Status);

            builder.Property(t => t.IsActive)
                .HasColumnName("Active");

            builder.HasOne(s => s.Supplier)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(s => s.IdSupplier);

            builder.HasOne(s => s.Store)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

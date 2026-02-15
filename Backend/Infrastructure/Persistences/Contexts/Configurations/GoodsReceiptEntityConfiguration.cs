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
            builder.Property(r => r.IdReceipt)
                .HasColumnName("PK_RECEIPT");

            builder.Property(r => r.Code)
                .HasColumnName("CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(r => r.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(r => r.DocumentDate)
                .HasColumnName("DOCUMENT_DATE")
                .IsRequired();

            builder.Property(r => r.DocumentType)
                .HasColumnName("DOCUMENT_TYPE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(r => r.DocumentNumber)
                .HasColumnName("DOCUMENT_NUMBER")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(r => r.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(r => r.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(r => r.IdSupplier)
                .HasColumnName("PK_SUPPLIER")
                .IsRequired();

            builder.Property(r => r.IdStore)
                .HasColumnName("PK_STORE")
                .IsRequired();

            builder.Property(r => r.AuditCreateUser)
                .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(r => r.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(r => r.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(r => r.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(r => r.Status)
                .HasColumnName("STATUS");

            builder.Property(t => t.IsActive)
                .HasColumnName("ACTIVE");

            builder.HasOne(s => s.Supplier)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(s => s.IdSupplier);

            builder.HasOne(s => s.Store)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

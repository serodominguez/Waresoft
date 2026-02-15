using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class TransferEntityConfiguration : IEntityTypeConfiguration<TransferEntity>
    {
        public void Configure(EntityTypeBuilder<TransferEntity> builder)
        {
            builder.ToTable("TRANSFERS");

            builder.HasKey(t => t.IdTransfer);
            builder.Property(t => t.IdTransfer)
                .HasColumnName("PK_TRANSFER");

            builder.Property(t => t.Code)
                .HasColumnName("CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(t => t.SendDate)
                .HasColumnName("SEND_DATE");

            builder.Property(t => t.ReceiveDate)
                .HasColumnName("RECEIVE_DATE");

            builder.Property(t => t.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(t => t.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(t => t.IdStoreOrigin)
                .HasColumnName("PK_STORE_ORIGIN")
                .IsRequired();

            builder.Property(t => t.IdStoreDestination)
                .HasColumnName("PK_STORE_DESTINATION")
                .IsRequired();

            builder.Property(t => t.AuditCreateUser)
              .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(t => t.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(t => t.AuditUpdateUser)
              .HasColumnName("AUDIT_UPDATE_USER");

            builder.Property(t => t.AuditUpdateDate)
                .HasColumnName("AUDIT_UPDATE_DATE");

            builder.Property(t => t.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(t => t.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(t => t.Status)
                .HasColumnName("STATUS");

            builder.Property(t => t.IsActive)
                .HasColumnName("ACTIVE");

            builder.HasOne(t => t.StoreOrigin)
                .WithMany(s => s.TransfersAsOrigin)
                .HasForeignKey(t => t.IdStoreOrigin);

            builder.HasOne(t => t.StoreDestination)
                .WithMany(s => s.TransfersAsDestination)
                .HasForeignKey(t => t.IdStoreDestination);
        }
    }
}

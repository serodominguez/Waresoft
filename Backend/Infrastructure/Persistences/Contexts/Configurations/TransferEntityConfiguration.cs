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
            builder.Property(t => t.IdTransfer);

            builder.Property(t => t.Code)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(t => t.SendDate);

            builder.Property(t => t.ReceiveDate);

            builder.Property(t => t.TotalAmount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(t => t.Annotations)
                .HasMaxLength(80);

            builder.Property(t => t.IdStoreOrigin)
                .IsRequired();

            builder.Property(t => t.IdStoreDestination)
                .IsRequired();

            builder.Property(t => t.AuditCreateUser);

            builder.Property(t => t.AuditCreateDate);

            builder.Property(t => t.AuditUpdateUser);

            builder.Property(t => t.AuditUpdateDate);

            builder.Property(t => t.AuditDeleteUser);

            builder.Property(t => t.AuditDeleteDate);

            builder.Property(t => t.Status);

            builder.Property(t => t.IsActive)
                .HasColumnName("Active");

            builder.HasOne(t => t.StoreOrigin)
                .WithMany(s => s.TransfersAsOrigin)
                .HasForeignKey(t => t.IdStoreOrigin);

            builder.HasOne(t => t.StoreDestination)
                .WithMany(s => s.TransfersAsDestination)
                .HasForeignKey(t => t.IdStoreDestination);
        }
    }
}

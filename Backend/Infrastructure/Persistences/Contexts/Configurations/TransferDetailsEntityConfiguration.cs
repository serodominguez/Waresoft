using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class TransferDetailsEntityConfiguration : IEntityTypeConfiguration<TransferDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<TransferDetailsEntity> builder)
        {
            builder.ToTable("TRANSFERS_DETAILS")
                .HasKey(d => new { d.IdTransfer, d.Item });

            builder.Property(d => d.IdTransfer)
                .IsRequired();

            builder.Property(d => d.Item)
                .IsRequired();

            builder.Property(d => d.IdProduct)
                .IsRequired();

            builder.Property(d => d.Quantity)
                .IsRequired();

            builder.Property(d => d.UnitPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.TotalPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.HasOne(t => t.Transfer)
                .WithMany(d => d.TransferDetails)
                .HasForeignKey(t => t.IdTransfer)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(d => d.TransferDetails)
                .HasForeignKey(p => p.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

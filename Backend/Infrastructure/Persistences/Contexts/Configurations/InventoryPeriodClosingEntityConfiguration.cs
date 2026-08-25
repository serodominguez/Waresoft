using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InventoryPeriodClosingEntityConfiguration : IEntityTypeConfiguration<InventoryPeriodClosingEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryPeriodClosingEntity> builder)
        {
            builder.ToTable("InventoryPeriodClosing")
                .HasKey(i => new { i.IdPeriod, i.IdProduct });

            builder.Property(i => i.IdPeriod)
                .IsRequired();

            builder.Property(i => i.IdProduct)
                .IsRequired();

            builder.Property(i => i.SystemStock)
                .IsRequired();

            builder.Property(i => i.PhysicalStock);

            builder.Property(i => i.Difference);

            builder.Property(i => i.ClosingStock)
                .IsRequired();

            builder.HasOne(p => p.InventoryPeriod)
                .WithMany(i => i.InventoryPeriodClosing)
                .HasForeignKey(i => i.IdPeriod)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Product)
                .WithMany(i => i.InventoryPeriodClosing)
                .HasForeignKey(i => i.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

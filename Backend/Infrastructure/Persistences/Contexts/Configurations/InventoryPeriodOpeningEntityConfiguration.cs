using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InventoryPeriodOpeningEntityConfiguration : IEntityTypeConfiguration<InventoryPeriodOpeningEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryPeriodOpeningEntity> builder)
        {
            builder.ToTable("InventoryPeriodOpening")
                .HasKey(i => new { i.IdPeriod, i.IdProduct });

            builder.Property(i => i.IdPeriod)
                .IsRequired();

            builder.Property(i => i.IdProduct)
                .IsRequired();

            builder.Property(i => i.OpeningStock)
                .IsRequired();

            builder.HasOne(p => p.InventoryPeriod)
                .WithMany(i => i.InventoryPeriodOpening)
                .HasForeignKey(i => i.IdPeriod)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Product)
                .WithMany(i => i.InventoryPeriodOpening)
                .HasForeignKey(i => i.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

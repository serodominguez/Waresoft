using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class StoreInventoryEntityConfiguration : IEntityTypeConfiguration<StoreInventoryEntity>
    {
        public void Configure(EntityTypeBuilder<StoreInventoryEntity> builder)
        {
            builder.ToTable("STORES_INVENTORY")
                .HasKey(i => new { i.IdStore, i.IdProduct });

            builder.Property(i => i.IdStore)
                .IsRequired();

            builder.Property(i => i.IdProduct)
                .IsRequired();

            builder.Property(i => i.StockAvailable)
                .IsRequired();

            builder.Property(i => i.StockInTransit)
                .IsRequired();

            builder.Property(i => i.MinimumStock)
                .IsRequired();

            builder.Property(i => i.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(i => i.AuditCreateUser);

            builder.Property(i => i.AuditCreateDate);

            builder.Property(i => i.AuditUpdateUser);

            builder.Property(i => i.AuditUpdateDate);

            builder.HasOne(s => s.Store)
                .WithMany(i => i.Inventory)
                .HasForeignKey(s => s.IdStore)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(i => i.Inventory)
                .HasForeignKey(p => p.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

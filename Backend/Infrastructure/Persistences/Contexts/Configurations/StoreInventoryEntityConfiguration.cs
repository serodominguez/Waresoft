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
                .HasColumnName("PK_STORE")
                .IsRequired();

            builder.Property(i => i.IdProduct)
                .HasColumnName("PK_PRODUCT")
                .IsRequired();

            builder.Property(i => i.StockAvailable)
                .HasColumnName("STOCK_AVAILABLE")
                .IsRequired();

            builder.Property(i => i.StockInTransit)
                .HasColumnName("STOCK_IN_TRANSIT")
                .IsRequired();

            builder.Property(i => i.Price)
                .HasColumnName("PRICE")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(i => i.AuditCreateUser)
                .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(i => i.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(i => i.AuditUpdateUser)
                .HasColumnName("AUDIT_UPDATE_USER");

            builder.Property(i => i.AuditUpdateDate)
                .HasColumnName("AUDIT_UPDATE_DATE");

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

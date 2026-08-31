using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InventoryPeriodEntityConfiguration : IEntityTypeConfiguration<InventoryPeriodEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryPeriodEntity> builder)
        {
            builder.ToTable("InventoryPeriods")
                .HasKey(i => i.IdPeriod);

            builder.Property(i => i.IdStore)
                .IsRequired();

            builder.Property(i => i.PeriodName)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(i => i.StartDate)
                .IsRequired();

            builder.Property(i => i.EndDate)
                .IsRequired();

            builder.Property(i => i.Status)
                .IsRequired();

            builder.Property(i => i.OpenedByUser)
                .IsRequired();

            builder.Property(i => i.OpenedDate)
                .IsRequired();

            builder.Property(i => i.ClosedByUser);

            builder.Property(i => i.ClosedDate);

            builder.HasOne(s => s.Store)
                .WithMany(i => i.InventoryPeriod)
                .HasForeignKey(i => i.IdStore)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

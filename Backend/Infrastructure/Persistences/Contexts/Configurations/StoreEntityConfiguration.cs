using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class StoreEntityConfiguration : BaseAuditEntityConfiguration<StoreEntity>
    {
        public override void Configure(EntityTypeBuilder<StoreEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("STORES");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .HasColumnName("IdStore");

            builder.Property(s => s.StoreName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.Manager)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(s => s.Address)
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(s => s.City)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(s => s.Email)
                .HasMaxLength(50);

            builder.Property(s => s.ProfitMargin)
                .IsRequired();

            builder.Property(s => s.Type)
                .HasMaxLength(15)
                .IsRequired();
        }
    }
}

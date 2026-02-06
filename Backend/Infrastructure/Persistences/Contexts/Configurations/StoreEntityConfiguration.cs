using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class StoreEntityConfiguration : BaseEntityConfiguration<StoreEntity>
    {
        public override void Configure(EntityTypeBuilder<StoreEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("STORES");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .HasColumnName("PK_STORE");

            builder.Property(s => s.StoreName)
                .HasColumnName("STORE_NAME")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.Manager)
                .HasColumnName("MANAGER")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(s => s.Address)
                .HasColumnName("ADDRESS")
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .HasColumnName("PHONE_NUMBER");

            builder.Property(s => s.City)
                .HasColumnName("CITY")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(s => s.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(50);

            builder.Property(s => s.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(15)
                .IsRequired();
        }
    }
}

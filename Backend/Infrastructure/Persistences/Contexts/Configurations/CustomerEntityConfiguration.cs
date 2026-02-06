using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class CustomerEntityConfiguration : BaseEntityConfiguration<CustomerEntity>
    {
        public override void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("CUSTOMERS");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("PK_CUSTOMER");

            builder.Property(c => c.Names)
                .HasColumnName("NAMES")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.LastNames)
                .HasColumnName("LAST_NAMES")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.IdentificationNumber)
                .HasColumnName("IDENTIFICATION_NUMBER")
                .HasMaxLength(10);

            builder.Property(c => c.PhoneNumber)
                .HasColumnName("PHONE_NUMBER");
        }

    }
}

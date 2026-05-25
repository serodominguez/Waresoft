using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class CustomerEntityConfiguration : BaseAuditEntityConfiguration<CustomerEntity>
    {
        public override void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("CUSTOMERS");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("IdCustomer");

            builder.Property(c => c.Names)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.LastNames)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.IdentificationNumber)
                .HasMaxLength(10);

            builder.Property(c => c.PhoneNumber);
        }

    }
}

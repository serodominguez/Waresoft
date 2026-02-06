using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class SupplierEntityConfiguration : BaseEntityConfiguration<SupplierEntity>
    {
        public override void Configure(EntityTypeBuilder<SupplierEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("SUPPLIERS");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .HasColumnName("PK_SUPPLIER");

            builder.Property(s => s.CompanyName)
                .HasColumnName("COMPANY_NAME")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.Contact)
                .HasColumnName("CONTACT")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .HasColumnName("PHONE_NUMBER");

            builder.Property(s => s.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(50);
        }
    }
}

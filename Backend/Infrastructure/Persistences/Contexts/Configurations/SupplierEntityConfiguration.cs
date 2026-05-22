using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class SupplierEntityConfiguration : BaseAuditEntityConfiguration<SupplierEntity>
    {
        public override void Configure(EntityTypeBuilder<SupplierEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("SUPPLIERS");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .HasColumnName("IdSupplier");

            builder.Property(s => s.CompanyName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.Contact)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(s => s.Email)
                .HasMaxLength(50);

            builder.Property(s => s.Status);
        }
    }
}

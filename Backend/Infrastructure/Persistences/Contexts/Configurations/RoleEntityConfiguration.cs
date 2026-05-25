using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class RoleEntityConfiguration : BaseAuditEntityConfiguration<RoleEntity>
    {
        public override void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("ROLES");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasColumnName("IdRole");

            builder.Property(r => r.RoleName)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}

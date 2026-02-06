using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class RoleEntityConfiguration : BaseEntityConfiguration<RoleEntity>
    {
        public override void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("ROLES");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasColumnName("PK_ROLE");

            builder.Property(r => r.RoleName)
                .HasColumnName("ROLE_NAME")
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}

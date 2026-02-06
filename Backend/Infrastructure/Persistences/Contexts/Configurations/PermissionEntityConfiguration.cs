using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class PermissionEntityConfiguration : BaseEntityConfiguration<PermissionEntity>
    {
        public override void Configure(EntityTypeBuilder<PermissionEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("PERMISSIONS");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .HasColumnName("PK_PERMISSION");

            builder.Property(p => p.IdRole)
                .HasColumnName("PK_ROLE")
                .IsRequired();

            builder.Property(p => p.IdModule)
                .HasColumnName("PK_MODULE")
                .IsRequired();

            builder.Property(p => p.IdAction)
                .HasColumnName("PK_ACTION")
                .IsRequired();

            builder.HasOne(r => r.Role)
                .WithMany(p => p.Permission)
                .HasForeignKey(r => r.IdRole);

            builder.HasOne(m => m.Module)
                .WithMany(p => p.Permission)
                .HasForeignKey(m => m.IdModule);

            builder.HasOne(a => a.Action)
                .WithMany(p => p.Permissions)
                .HasForeignKey(a => a.IdAction);


            builder.HasIndex(p => new { p.IdRole, p.IdModule, p.IdAction })
                .IsUnique()
                .HasDatabaseName("IX_Permissions_Role_Module_Action");

            builder.HasIndex(p => p.IdRole)
                .HasDatabaseName("IX_Permissions_Role");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_Permissions_Status");
        }
    }
}

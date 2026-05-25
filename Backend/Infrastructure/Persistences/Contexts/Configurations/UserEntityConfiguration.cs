using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class UserEntityConfiguration : BaseAuditEntityConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("USERS");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("IdUser");

            builder.Property(u => u.UserName)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.PasswordHash);

            builder.Property(u => u.PasswordSalt);

            builder.Property(u => u.Names)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(u => u.LastNames)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.IdentificationNumber)
                .HasMaxLength(10);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(u => u.IdRole);

            builder.Property(u => u.IdStore);

            builder.HasOne(r => r.Role)
                .WithMany(u => u.User)
                .HasForeignKey(u => u.IdRole);

            builder.HasOne(s => s.Store)
                .WithMany(u => u.User)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

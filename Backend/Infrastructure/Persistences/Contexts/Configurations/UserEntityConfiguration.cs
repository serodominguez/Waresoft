using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class UserEntityConfiguration : BaseEntityConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("USERS");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("PK_USER");

            builder.Property(u => u.UserName)
                .HasColumnName("USER_NAME")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("PASSWORD_HASH");

            builder.Property(u => u.PasswordSalt)
                .HasColumnName("PASSWORD_SALT");

            builder.Property(u => u.Names)
                .HasColumnName("NAMES")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(u => u.LastNames)
                .HasColumnName("LAST_NAMES")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.IdentificationNumber)
                .HasColumnName("IDENTIFICATION_NUMBER")
                .HasMaxLength(10);

            builder.Property(u => u.PhoneNumber)
                .HasColumnName("PHONE_NUMBER");

            builder.Property(u => u.IdRole)
                .HasColumnName("PK_ROLE");

            builder.Property(u => u.IdStore)
                .HasColumnName("PK_STORE");

            builder.HasOne(r => r.Role)
                .WithMany(u => u.User)
                .HasForeignKey(u => u.IdRole);

            builder.HasOne(s => s.Store)
                .WithMany(u => u.User)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

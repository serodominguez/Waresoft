using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.AuditCreateUser)
                .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(e => e.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(e => e.AuditUpdateUser)
                .HasColumnName("AUDIT_UPDATE_USER");

            builder.Property(e => e.AuditUpdateDate)
                .HasColumnName("AUDIT_UPDATE_DATE");

            builder.Property(e => e.AuditDeleteUser)
                .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(e => e.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(e => e.Status)
                .HasColumnName("STATUS");
        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.AuditCreateUser);

            builder.Property(e => e.AuditCreateDate);

            builder.Property(e => e.AuditUpdateUser);

            builder.Property(e => e.AuditUpdateDate);

            builder.Property(e => e.AuditDeleteUser);

            builder.Property(e => e.AuditDeleteDate);

            builder.Property(e => e.Status);
        }
    }
}

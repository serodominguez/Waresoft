using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public abstract class BaseAuditEntityConfiguration<T> : BaseEntityConfiguration<T> where T : BaseAuditEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.AuditUpdateUser);
            builder.Property(e => e.AuditUpdateDate);
        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ActionEntityConfiguration : BaseAuditEntityConfiguration<ActionEntity>
    {
        public override void Configure(EntityTypeBuilder<ActionEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("ACTIONS");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .HasColumnName("IdAction");

            builder.Property(a => a.ActionName)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}

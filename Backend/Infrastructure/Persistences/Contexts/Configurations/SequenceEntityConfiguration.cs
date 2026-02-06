using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class SequenceEntityConfiguration : IEntityTypeConfiguration<SequenceEntity>
    {
        public void Configure(EntityTypeBuilder<SequenceEntity> builder)
        {
            builder.ToTable("SEQUENCES");

            builder.HasKey(s => s.Name);

            builder.Property(s => s.Name)
                .HasColumnName("NAME")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.CurrentValue)
                .HasColumnName("CURRENT_VALUE")
                .IsRequired();

            builder.Property(s => s.LastUpdated)
                .HasColumnName("LAST_UPDATED");
        }
    }
}

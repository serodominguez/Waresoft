using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class SequenceEntityConfiguration : IEntityTypeConfiguration<SequenceEntity>
    {
        public void Configure(EntityTypeBuilder<SequenceEntity> builder)
        {
            builder.ToTable("SEQUENCES")
                .HasKey(s => new { s.Name, s.IdStore });

            builder.Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.IdStore)
                .IsRequired();

            builder.Property(s => s.CurrentValue)
                .IsRequired();

            builder.Property(s => s.LastUpdated);
        }
    }
}

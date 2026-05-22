using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class CategoryEntityConfiguration : BaseAuditEntityConfiguration<CategoryEntity>
    {
        public override void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("CATEGORIES");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("IdCategory");

            builder.Property(c => c.CategoryName)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(50);

            builder.Property(c => c.Status);
        }
    }
}

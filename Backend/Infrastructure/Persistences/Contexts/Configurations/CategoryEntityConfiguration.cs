using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class CategoryEntityConfiguration : BaseEntityConfiguration<CategoryEntity>
    {
        public override void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("CATEGORIES");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("PK_CATEGORY");

            builder.Property(c => c.CategoryName)
                .HasColumnName("CATEGORY_NAME")
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(50);
        }
    }
}

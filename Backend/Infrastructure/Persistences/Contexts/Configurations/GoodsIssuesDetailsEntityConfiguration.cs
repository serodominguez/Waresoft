using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesDetailsEntityConfiguration : IEntityTypeConfiguration<GoodsIssueDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsIssueDetailsEntity> builder)
        {
            builder.ToTable("GOODS_ISSUE_DETAILS")
                            .HasKey(d => new { d.IdIssue, d.Item });

            builder.Property(d => d.IdIssue)
                .IsRequired();

            builder.Property(d => d.Item)
                .IsRequired();

            builder.Property(d => d.IdProduct)
                .IsRequired();

            builder.Property(d => d.Quantity)
                .IsRequired();

            builder.Property(d => d.UnitPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.TotalPrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.HasOne(i => i.GoodsIssue)
                .WithMany(d => d.GoodsIssueDetails)
                .HasForeignKey(i => i.IdIssue)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(d => d.GoodsIssueDetails)
                .HasForeignKey(p => p.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

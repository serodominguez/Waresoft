using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesEntityConfiguration : BaseEntityConfiguration<GoodsIssueEntity>
    {
        public override void Configure(EntityTypeBuilder<GoodsIssueEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("GOODS_ISSUE");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .HasColumnName("IdIssue");

            builder.Property(i => i.Code)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(i => i.Type)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(i => i.TotalAmount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(i => i.Annotations)
                .HasMaxLength(80);

            builder.Property(i => i.IdUser);

            builder.Property(i => i.IdStore)
                .IsRequired();

            builder.Property(i => i.Status);

            builder.Property(t => t.IsActive)
                .HasColumnName("Active");

            builder.HasOne(u => u.User)
                .WithMany(i => i.GoodsIssue)
                .HasForeignKey(u => u.IdUser);

            builder.HasOne(s => s.Store)
                .WithMany(i => i.GoodsIssue)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

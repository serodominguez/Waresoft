using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesEntityConfiguration : IEntityTypeConfiguration<GoodsIssueEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsIssueEntity> builder)
        {
            builder.ToTable("GOODS_ISSUE");

            builder.HasKey(i => i.IdIssue);
            builder.Property(i => i.IdIssue)
                .HasColumnName("PK_ISSUE");

            builder.Property(i => i.Code)
                .HasColumnName("CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(i => i.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(i => i.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(i => i.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(i => i.IdUser)
                .HasColumnName("PK_USER")
                .IsRequired();

            builder.Property(i => i.IdStore)
                .HasColumnName("PK_STORE")
                .IsRequired();

            builder.Property(i => i.AuditCreateUser)
                .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(i => i.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(i => i.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(i => i.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(i => i.Status)
                .HasColumnName("STATUS");

            builder.HasOne(u => u.User)
                .WithMany(i => i.GoodsIssue)
                .HasForeignKey(u => u.IdUser);

            builder.HasOne(s => s.Store)
                .WithMany(i => i.GoodsIssue)
                .HasForeignKey(s => s.IdStore);
        }
    }
}

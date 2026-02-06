using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistences.Contexts
{
    public partial class DbContextSystem : DbContext
    {
        public DbContextSystem(DbContextOptions<DbContextSystem> options) : base(options) { }

        public virtual DbSet<ActionEntity> Action { get; set; }
        public virtual DbSet<BrandEntity> Brand { get; set; }
        public virtual DbSet<CategoryEntity> Category { get; set; }
        public virtual DbSet<CustomerEntity> Customer { get; set; }
        public virtual DbSet<GoodsIssueEntity> GoodsIssue { get; set; } = null!;
        public virtual DbSet<GoodsIssueDetailsEntity> GoodsIssueDetails { get; set; }
        public virtual DbSet<GoodsReceiptEntity> GoodsReceipt { get; set; }
        public virtual DbSet<GoodsReceiptDetailsEntity> GoodsReceiptDetails { get; set; }
        public virtual DbSet<ModuleEntity> Module { get; set; }
        public virtual DbSet<PermissionEntity> Permission { get; set; }
        public virtual DbSet<ProductEntity> Product { get; set; }
        public virtual DbSet<RoleEntity> Role { get; set; }
        public virtual DbSet<SequenceEntity> Sequence { get; set; }
        public virtual DbSet<StoreInventoryEntity> StoreInventory { get; set; }
        public virtual DbSet<StoreEntity> Store { get; set; }
        public virtual DbSet<SupplierEntity> Supplier { get; set; }
        public virtual DbSet<TransferEntity> Transfer { get; set; }
        public virtual DbSet<TransferDetailsEntity> TransferDetails { get; set; }
        public virtual DbSet<UserEntity> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

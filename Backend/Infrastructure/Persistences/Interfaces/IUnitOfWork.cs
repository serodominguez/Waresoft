using Domain.Entities;
using System.Data;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ActionEntity> Action { get; }
        IGenericRepository<BrandEntity> Brand { get; }
        IGenericRepository<CategoryEntity> Category { get; }
        IGenericRepository<CustomerEntity> Customer { get; }
        IGenericRepository<ModuleEntity> Module { get; }
        IGenericRepository<RoleEntity> Role { get; }
        IGenericRepository<StoreEntity> Store { get; }
        IGenericRepository<SupplierEntity> Supplier { get; }

        IGoodsIssueDetailsRepository GoodsIssueDetails { get; }
        IGoodsIssueRepository GoodsIssue { get; }
        IGoodsReceiptDetailsRepository GoodsReceiptDetails { get; }
        IGoodsReceiptRepository GoodsReceipt { get; }
        IPermissionRepository Permission { get; }
        IProductRepository Product { get; }
        IStoreInventoryRepository StoreInventory { get; }
        ITransferDetailsRepository TransferDetails { get; }
        ITransferRepository Transfer { get; }
        IUserRepository User { get; }

        IDbTransaction BeginTransaction();
        void SaveChanges();
        Task<int> SaveChangesAsync();
    }
}

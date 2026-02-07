using Domain.Entities;
using System.Data;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<BrandEntity> Brand { get; }
        IGenericRepository<CategoryEntity> Category { get; }
        IGenericRepository<CustomerEntity> Customer { get; }
        IGenericRepository<StoreEntity> Store { get; }
        IGenericRepository<SupplierEntity> Supplier { get; }

        IActionRepository Action { get; }
        IGoodsIssueDetailsRepository GoodsIssueDetails { get; }
        IGoodsIssueRepository GoodsIssue { get; }
        IGoodsReceiptDetailsRepository GoodsReceiptDetails { get; }
        IGoodsReceiptRepository GoodsReceipt { get; }
        IModuleRepository Module { get; }
        IPermissionRepository Permission { get; }
        IProductRepository Product { get; }
        IRoleRepository Role { get; }
        IStoreInventoryRepository StoreInventory { get; }
        ITransferDetailsRepository TransferDetails { get; }
        ITransferRepository Transfer { get; }
        IUserRepository User { get; }

        IDbTransaction BeginTransaction();
        void SaveChanges();
        Task SaveChangesAsync();
    }
}

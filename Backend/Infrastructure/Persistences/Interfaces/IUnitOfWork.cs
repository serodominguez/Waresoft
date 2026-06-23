using Domain.Entities;
using System.Data;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IActionQueryRepository ActionQuery { get; }
        IBrandQueryRepository BrandQuery { get; }
        ICategoryQueryRepository CategoryQuery { get; }
        ICustomerQueryRepository CustomerQuery { get; }
        IDashboardQueryRepository DashboardQuery { get; }
        IGoodsIssueDetailsQueryRepository GoodsIssueDetailsQuery { get; }
        IGoodsIssueQueryRepository GoodsIssueQuery { get; }
        IGoodsReceiptDetailsQueryRepository GoodsReceiptDetailsQuery { get; }
        IGoodsReceiptQueryRepository GoodsReceiptQuery { get; }
        IModuleQueryRepository ModuleQuery { get; }
        IPermissionQueryRepository PermissionQuery { get; }
        IProductQueryRepository ProductQuery { get; }
        ISequenceQueryRepository SequenceQuery { get; }
        IStoreInventoryQueryRepository StoreInventoryQuery { get; }
        IStoreQueryRepository StoreQuery { get; }
        ISupplierQueryRepository SupplierQuery { get; }
        IRoleQueryRepository RoleQuery { get; }
        ITransferDetailsQueryRepository TransferDetailsQuery { get; }
        ITransferQueryRepository TransferQuery { get; }
        IUserQueryRepository UserQuery { get; }

        IGenericRepository<BrandEntity> BrandCommand { get; }
        IGenericRepository<CategoryEntity> CategoryCommand { get; }
        IGenericRepository<CustomerEntity> CustomerCommand { get; }
        IGenericRepository<GoodsIssueEntity> GoodsIssueCommand { get; }
        IGenericRepository<GoodsReceiptEntity> GoodsReceiptCommand { get; }
        IGenericRepository<ModuleEntity> ModuleCommand { get; } 
        IGenericRepository<PermissionEntity> PermissionCommand { get; }
        IGenericRepository<ProductEntity> ProductCommand { get; }
        ISequenceCommandRepository SequenceCommand { get; }
        IStoreInventoryCommandRepository StoreInventoryCommand { get; }
        IGenericRepository<StoreEntity> StoreCommand { get; }
        IGenericRepository<SupplierEntity> SupplierCommand { get; }
        IGenericRepository<RoleEntity> RoleCommand { get; } 
        IGenericRepository<TransferEntity> TransferCommand { get; }
        IGenericRepository<UserEntity> UserCommand { get; }

        IDbTransaction BeginTransaction();
        void SaveChanges();
        Task<int> SaveChangesAsync();
    }
}

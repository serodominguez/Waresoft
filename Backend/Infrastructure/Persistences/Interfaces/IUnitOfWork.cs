using Domain.Entities;
using Infrastructure.Persistences.Interfaces.Action;
using Infrastructure.Persistences.Interfaces.Brand;
using Infrastructure.Persistences.Interfaces.Category;
using Infrastructure.Persistences.Interfaces.Customer;
using Infrastructure.Persistences.Interfaces.Dashboard;
using Infrastructure.Persistences.Interfaces.GoodsIssue;
using Infrastructure.Persistences.Interfaces.GoodsReceipt;
using Infrastructure.Persistences.Interfaces.Module;
using Infrastructure.Persistences.Interfaces.Permission;
using Infrastructure.Persistences.Interfaces.Product;
using Infrastructure.Persistences.Interfaces.Role;
using Infrastructure.Persistences.Interfaces.Sequence;
using Infrastructure.Persistences.Interfaces.Store;
using Infrastructure.Persistences.Interfaces.StoreInventory;
using Infrastructure.Persistences.Interfaces.Supplier;
using Infrastructure.Persistences.Interfaces.Transfer;
using Infrastructure.Persistences.Interfaces.User;
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

using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Interfaces.Action;
using Infrastructure.Persistences.Interfaces.Brand;
using Infrastructure.Persistences.Interfaces.Category;
using Infrastructure.Persistences.Interfaces.Customer;
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
using Infrastructure.Persistences.Repositories.Action;
using Infrastructure.Persistences.Repositories.Brand;
using Infrastructure.Persistences.Repositories.Category;
using Infrastructure.Persistences.Repositories.Customer;
using Infrastructure.Persistences.Repositories.GoodsIssue;
using Infrastructure.Persistences.Repositories.GoodsReceipt;
using Infrastructure.Persistences.Repositories.Module;
using Infrastructure.Persistences.Repositories.Permission;
using Infrastructure.Persistences.Repositories.Product;
using Infrastructure.Persistences.Repositories.Role;
using Infrastructure.Persistences.Repositories.Sequence;
using Infrastructure.Persistences.Repositories.Store;
using Infrastructure.Persistences.Repositories.StoreInventory;
using Infrastructure.Persistences.Repositories.Supplier;
using Infrastructure.Persistences.Repositories.Transfer;
using Infrastructure.Persistences.Repositories.User;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextSystem _context;

        private IActionQueryRepository _actionQuery = null!;
        private IBrandQueryRepository _brandQuery = null!;
        private ICategoryQueryRepository _categoryQuery = null!;
        private ICustomerQueryRepository _customerQuery = null!;
        private IGoodsIssueDetailsQueryRepository _goodsIssueDetailsQuery = null!;
        private IGoodsIssueQueryRepository _goodsIssueQuery = null!;
        private IGoodsReceiptDetailsQueryRepository _goodsReceiptDetailsQuery = null!;
        private IGoodsReceiptQueryRepository _goodsReceiptQuery = null!;
        private IModuleQueryRepository _moduleQuery = null!;
        private IPermissionQueryRepository _permissionQuery = null!;
        private IProductQueryRepository _productQuery = null!;
        private ISequenceQueryRepository _sequenceQuery = null!;
        private IStoreInventoryQueryRepository _storeInventoryQuery = null!;
        private IStoreQueryRepository _storeQuery = null!;
        private ISupplierQueryRepository _supplierQuery = null!;
        private IRoleQueryRepository _roleQuery = null!;
        private ITransferDetailsQueryRepository _transferDetailsQuery = null!;
        private ITransferQueryRepository _transferQuery = null!;
        private IUserQueryRepository _userQuery = null!;

        
        private IGenericRepository<BrandEntity> _brandCommand = null!;
        private IGenericRepository<CategoryEntity> _categoryCommand = null!;
        private IGenericRepository<CustomerEntity> _customerCommand = null!;
        private IGenericRepository<GoodsIssueEntity> _goodsIssueCommand = null!;
        private IGenericRepository<GoodsReceiptEntity> _goodsReceiptCommand = null!;
        private IGenericRepository<ModuleEntity> _moduleCommand = null!;
        private IGenericRepository<PermissionEntity> _permissionCommand = null!;
        private IGenericRepository<ProductEntity> _productCommand = null!;
        private ISequenceCommandRepository _sequenceCommand = null!;
        private IStoreInventoryCommandRepository _storeInventoryCommand = null!;
        private IGenericRepository<StoreEntity> _storeCommand = null!;
        private IGenericRepository<SupplierEntity> _supplierCommand = null!;
        private IGenericRepository<RoleEntity> _roleCommand = null!;
        private IGenericRepository<TransferEntity> _transferCommand = null!;
        private IGenericRepository<UserEntity> _userCommand = null!;

        public UnitOfWork(DbContextSystem context)
        {
            _context = context;
        }

        public IActionQueryRepository ActionQuery => _actionQuery ?? new ActionQueryRepository(_context);
        public IBrandQueryRepository BrandQuery => _brandQuery ?? new BrandQueryRepository(_context);
        public ICategoryQueryRepository CategoryQuery => _categoryQuery ?? new CategoryQueryRepository(_context);
        public ICustomerQueryRepository CustomerQuery => _customerQuery ?? new CustomerQueryRepository(_context);
        public IGoodsIssueDetailsQueryRepository GoodsIssueDetailsQuery => _goodsIssueDetailsQuery ?? new GoodsIssueDetailsQueryRepository(_context);
        public IGoodsIssueQueryRepository GoodsIssueQuery => _goodsIssueQuery ?? new GoodsIssueQueryRepository(_context);
        public IGoodsReceiptDetailsQueryRepository GoodsReceiptDetailsQuery => _goodsReceiptDetailsQuery ?? new GoodsReceiptDetailsQueryRepository(_context);
        public IGoodsReceiptQueryRepository GoodsReceiptQuery => _goodsReceiptQuery ?? new GoodsReceiptQueryRepository(_context);
        public IModuleQueryRepository ModuleQuery => _moduleQuery ?? new ModuleQueryRepository(_context);
        public IPermissionQueryRepository PermissionQuery => _permissionQuery ?? new PermissionQueryRepository(_context);
        public IProductQueryRepository ProductQuery => _productQuery ?? new ProductQueryRepository(_context);
        public ISequenceQueryRepository SequenceQuery => _sequenceQuery ?? new SequenceQueryRepository(_context);
        public IStoreInventoryQueryRepository StoreInventoryQuery => _storeInventoryQuery ?? new StoreInventoryQueryRepository(_context);
        public IStoreQueryRepository StoreQuery => _storeQuery ?? new StoreQueryRepository(_context);
        public ISupplierQueryRepository SupplierQuery => _supplierQuery ?? new SupplierQueryRepository(_context);
        public IRoleQueryRepository RoleQuery => _roleQuery ?? new RoleQueryRepository(_context);
        public ITransferDetailsQueryRepository TransferDetailsQuery => _transferDetailsQuery ?? new TransferDetailsQueryRepository(_context);
        public ITransferQueryRepository TransferQuery => _transferQuery ?? new TransferQueryRepository(_context);
        public IUserQueryRepository UserQuery => _userQuery ?? new UserQueryRepository(_context);


        public IGenericRepository<BrandEntity> BrandCommand => _brandCommand ?? new GenericRepository<BrandEntity>(_context);
        public IGenericRepository<CategoryEntity> CategoryCommand => _categoryCommand ?? new GenericRepository<CategoryEntity>(_context);
        public IGenericRepository<CustomerEntity> CustomerCommand => _customerCommand ?? new GenericRepository<CustomerEntity>(_context);
        public IGenericRepository<GoodsIssueEntity> GoodsIssueCommand => _goodsIssueCommand ?? new GenericRepository<GoodsIssueEntity>(_context);
        public IGenericRepository<GoodsReceiptEntity> GoodsReceiptCommand => _goodsReceiptCommand ?? new GenericRepository<GoodsReceiptEntity>(_context);
        public IGenericRepository<ModuleEntity> ModuleCommand => _moduleCommand ?? new GenericRepository<ModuleEntity>(_context);
        public IGenericRepository<PermissionEntity> PermissionCommand => _permissionCommand ?? new GenericRepository<PermissionEntity>(_context);
        public IGenericRepository<ProductEntity> ProductCommand => _productCommand ?? new GenericRepository<ProductEntity>(_context);
        public ISequenceCommandRepository SequenceCommand => _sequenceCommand ?? new SequenceCommandRepository(_context);
        public IStoreInventoryCommandRepository StoreInventoryCommand => _storeInventoryCommand ?? new StoreInventoryCommandRepository(_context);
        public IGenericRepository<StoreEntity> StoreCommand => _storeCommand ?? new GenericRepository<StoreEntity>(_context);
        public IGenericRepository<SupplierEntity> SupplierCommand => _supplierCommand ?? new GenericRepository<SupplierEntity>(_context);
        public IGenericRepository<RoleEntity> RoleCommand => _roleCommand ?? new GenericRepository<RoleEntity>(_context);
        public IGenericRepository<TransferEntity> TransferCommand => _transferCommand ?? new GenericRepository<TransferEntity>(_context);
        public IGenericRepository<UserEntity> UserCommand => _userCommand ?? new GenericRepository<UserEntity>(_context);

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();
            return transaction.GetDbTransaction();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

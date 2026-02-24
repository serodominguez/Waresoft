using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextSystem _context;

        public IGenericRepository<ActionEntity> _action = null!;
        public IGenericRepository<BrandEntity> _brand = null!;
        public IGenericRepository<CategoryEntity> _category = null!;
        public IGenericRepository<CustomerEntity> _customer = null!;
        public IGenericRepository<ModuleEntity> _module = null!;
        public IGenericRepository<RoleEntity> _role = null!;
        public IGenericRepository<StoreEntity> _store = null!;
        public IGenericRepository<SupplierEntity> _supplier = null!;

        public IGoodsIssueDetailsRepository _goodsIssueDetails = null!;
        public IGoodsIssueRepository _goodsIssue = null!;
        public IGoodsReceiptDetailsRepository _goodsReceiptDetails = null!;
        public IGoodsReceiptRepository _goodsReceipt = null!;
        public IPermissionRepository _permission = null!;
        public IProductRepository _product = null!;
        public IStoreInventoryRepository _storeInventory = null!;
        public ITransferDetailsRepository _transferDetails = null!;
        public ITransferRepository _transfer = null!;
        public IUserRepository _user = null!;

        public UnitOfWork(DbContextSystem context)
        {
            _context = context;
        }

        public IGenericRepository<ActionEntity> Action => _action ?? new GenericRepository<ActionEntity>(_context);
        public IGenericRepository<BrandEntity> Brand => _brand ?? new GenericRepository<BrandEntity>(_context);
        public IGenericRepository<CategoryEntity> Category => _category ?? new GenericRepository<CategoryEntity>(_context);
        public IGenericRepository<CustomerEntity> Customer => _customer ?? new GenericRepository<CustomerEntity>(_context);
        public IGenericRepository<ModuleEntity> Module => _module ?? new GenericRepository<ModuleEntity>(_context);
        public IGenericRepository<RoleEntity> Role => _role ?? new GenericRepository<RoleEntity>(_context);
        public IGenericRepository<StoreEntity> Store => _store ?? new GenericRepository<StoreEntity>(_context);
        public IGenericRepository<SupplierEntity> Supplier => _supplier ?? new GenericRepository<SupplierEntity>(_context);

        public IGoodsIssueDetailsRepository GoodsIssueDetails => _goodsIssueDetails ?? new GoodsIssueDetailsRepository(_context);
        public IGoodsIssueRepository GoodsIssue => _goodsIssue ?? new GoodsIssueRepository(_context);
        public IGoodsReceiptDetailsRepository GoodsReceiptDetails => _goodsReceiptDetails ?? new GoodsReceiptDetailsRepository(_context);
        public IGoodsReceiptRepository GoodsReceipt => _goodsReceipt ?? new GoodsReceiptRepository(_context);
        public IPermissionRepository Permission => _permission ?? new PermissionRepository(_context);
        public IProductRepository Product => _product ?? new ProductRepository(_context);   
        public IStoreInventoryRepository StoreInventory => _storeInventory ?? new StoreInventoryRepository(_context);
        public ITransferDetailsRepository TransferDetails => _transferDetails ?? new TransferDetailsRepository(_context);
        public ITransferRepository Transfer => _transfer ?? new TransferRepository(_context);
        public IUserRepository User => _user ?? new UserRepository(_context);

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

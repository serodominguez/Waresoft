using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class StoreInventoryRepository : IStoreInventoryRepository
    {
        private readonly DbContextSystem _context;

        public StoreInventoryRepository(DbContextSystem context)
        {
            _context = context;
        }
        public IQueryable<StoreInventoryEntity> GetAllInventoryQueryable()
        {
            return _context.StoreInventory
                    .Select(i => new StoreInventoryEntity
                    {
                        IdStore = i.IdStore,
                        IdProduct = i.IdProduct,
                        StockAvailable = i.StockAvailable,
                        Store = new StoreEntity
                        {
                            Id = i.IdStore,
                            StoreName = i.Store.StoreName
                        },
                        Product = new ProductEntity
                        {
                            Id = i.Product.Id,
                            Code = i.Product.Code,
                            Description = i.Product.Description,
                            Color = i.Product.Color,
                            Material = i.Product.Material,
                            Brand = new BrandEntity
                            {
                                Id = i.Product.Brand.Id,
                                BrandName = i.Product.Brand.BrandName
                            },
                            Category = new CategoryEntity
                            {
                                Id = i.Product.Category.Id,
                                CategoryName = i.Product.Category.CategoryName
                            },
                            Status = i.Product.Status,
                            AuditCreateDate = i.Product.AuditCreateDate
                        }
                    });
        }

        public IQueryable<StoreInventoryEntity> GetInventoryQueryable(int storeId)
        {
            return _context.StoreInventory
                    .Where(i => i.IdStore == storeId)
                    .Select(i => new StoreInventoryEntity
                    {
                        IdStore = i.IdStore,
                        IdProduct = i.IdProduct,
                        StockAvailable = i.StockAvailable,
                        StockInTransit = i.StockInTransit,
                        Price = i.Price,
                        Store = new StoreEntity
                        {
                            StoreName = i.Store.StoreName
                        },
                        Product = new ProductEntity
                        {
                            Replenishment = i.Product.Replenishment,
                            Code = i.Product.Code,
                            Description = i.Product.Description,
                            Material = i.Product.Material,
                            Color = i.Product.Color,
                            UnitMeasure = i.Product.UnitMeasure,
                            Brand = new BrandEntity
                            {
                                BrandName = i.Product.Brand.BrandName
                            },
                            Category = new CategoryEntity
                            {
                                CategoryName = i.Product.Category.CategoryName
                            },
                            Status = i.Product.Status,
                            AuditCreateDate = i.Product.AuditCreateDate,
                            AuditDeleteUser = i.Product.AuditDeleteUser,
                            AuditDeleteDate = i.Product.AuditDeleteDate
                        }
                    });
        }

        public async Task<StoreInventoryEntity> GetStockByIdAsync(int productId, int storeId)
        {
            var stock = await _context.StoreInventory
                .FirstOrDefaultAsync(x => x.IdProduct == productId && x.IdStore == storeId);

            return stock!;
        }

        public async Task<bool> RegisterStockByProductsAsync(StoreInventoryEntity entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> UpdateStockByProductsAsync(StoreInventoryEntity entity)
        {
            _context.Update(entity);
            _context.Entry(entity).Property(x => x.Price).IsModified = false;
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}

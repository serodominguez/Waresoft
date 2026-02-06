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

        public IQueryable<StoreInventoryEntity> GetInventoryQueryable(int storeId)
        {
            return _context.StoreInventory
                .AsNoTracking()
                .Where(i => i.IdStore == storeId)
                .Include(i => i.Product.Brand)
                .Include(i => i.Product.Category);
        }

        public async Task<StoreInventoryEntity> GetStockByIdAsync(int productId, int storeId)
        {
            var stock = await _context.StoreInventory
                .AsNoTracking()
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

        public async Task<bool> UpdatePriceByProductsAsync(StoreInventoryEntity entity)
        {
            _context.Update(entity);
            _context.Entry(entity).Property(x => x.StockAvailable).IsModified = false;
            _context.Entry(entity).Property(x => x.StockInTransit).IsModified = false;
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}

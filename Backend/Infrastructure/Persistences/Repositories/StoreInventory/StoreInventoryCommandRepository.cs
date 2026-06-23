using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class StoreInventoryCommandRepository : IStoreInventoryCommandRepository
    {
        private readonly DbContextSystem _context;

        public StoreInventoryCommandRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task AddStoreInventoryAsync(StoreInventoryEntity entity)
        {
            await _context.AddAsync(entity);
        }

        public IQueryable<StoreInventoryEntity> GetStocksByStoreAsQueryable(int storeId)
        {
            return _context.StoreInventory
                .Where(s => s.IdStore == storeId);
        }

        public IQueryable<StoreInventoryEntity> GetStockByIdAsQueryable(int productId, int storeId)
        {
            return _context.StoreInventory
                .Where(x => x.IdProduct == productId && x.IdStore == storeId);
        }
    }
}

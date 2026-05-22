using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces.StoreInventory
{
    public interface IStoreInventoryCommandRepository
    {
        IQueryable<StoreInventoryEntity> GetStocksByStoreAsQueryable(int storeId);
        IQueryable<StoreInventoryEntity> GetStockByIdAsQueryable(int productId, int storeId);
        Task AddStoreInventoryAsync(StoreInventoryEntity entity);
    }
}

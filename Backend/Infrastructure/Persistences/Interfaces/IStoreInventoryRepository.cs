using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStoreInventoryRepository
    {
        IQueryable<StoreInventoryEntity> GetAllInventoryQueryable();
        IQueryable<StoreInventoryEntity> GetInventoryQueryable(int storeId);
        Task<StoreInventoryEntity> GetStockByIdAsync(int productId, int storeId);
        Task<bool> RegisterStockByProductsAsync(StoreInventoryEntity entity);
        Task<bool> UpdateStockByProductsAsync(StoreInventoryEntity entity);
    }
}

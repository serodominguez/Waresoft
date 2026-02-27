using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStoreInventoryRepository
    {
        IQueryable<StoreInventoryEntity> GetAllInventoryQueryable();
        IQueryable<StoreInventoryEntity> GetInventoryQueryable(int storeId);
        IQueryable<StoreInventoryEntity> GetStockByIdAsQueryable(int productId, int storeId);
        Task AddStoreInventoryAsync(StoreInventoryEntity entity);
    }
}

using Infrastructure.Persistences.ReadModels.InventoryPeriod;

namespace Infrastructure.Persistences.Interfaces.InventoryPeriod
{
    public interface IInventoryPeriodQueryRepository
    {
        IQueryable<InventoryPeriodReadModel> GetPeriodListQueryable(int storeId);
        Task<InventoryPeriodDetailReadModel?> GetPeriodDetailAsync(int periodId);
        Task<InventoryPeriodOpeningReadModel?> GetOpeningByPeriodAsync(int periodId);
        Task<InventoryPeriodClosingReadModel?> GetClosingByPeriodAsync(int periodId);
        Task<List<InventoryPeriodOpeningItemReadModel>> GetOpeningItemByPeriodAsync(int periodId);
        Task<List<InventoryPeriodClosingItemReadModel>> GetClosingItemByPeriodAsync(int periodId);
        Task<List<InventoryPeriodClosingItemReadModel>> CalculateSystemStockAsync(int periodId);
    }
}

using Infrastructure.Persistences.ReadModels.InventoryPeriod;

namespace Infrastructure.Persistences.Interfaces.InventoryPeriod
{
    public interface IInventoryPeriodQueryRepository
    {
        IQueryable<InventoryPeriodReadModel> GetPeriodListQueryable(int storeId);
        Task<InventoryPeriodDetailReadModel?> GetPeriodDetailAsync(int periodId);
        Task<List<InventoryPeriodClosingReadModel>> GetClosingByPeriodAsync(int periodId);
        Task<List<InventoryPeriodOpeningReadModel>> GetOpeningByPeriodAsync(int periodId);
        Task<List<InventoryPeriodClosingReadModel>> CalculateSystemStockAsync(int periodId);
    }
}

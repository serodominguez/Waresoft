using Domain.Entities;
using Domain.Models;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStoreInventoryRepository
    {
        IQueryable<StoreInventoryEntity> GetInventoryQueryable(int storeId);
        Task<List<KardexMovementModel>> GetKardexByProductAsync(int storeId, int productId, DateTime? startDate, DateTime? endDate);
        Task<(List<StoreInventoryModel> Data, int TotalRecords)> GetInventoryListAsync(int storeId, int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
        Task<(List<InventoryPivotModel> Data, int TotalRecords)> GetInventoryPivotAsync(int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
        IQueryable<StoreInventoryEntity> GetStocksByStoreAsQueryable(int storeId);
        IQueryable<StoreInventoryEntity> GetStockByIdAsQueryable(int productId, int storeId);
        Task AddStoreInventoryAsync(StoreInventoryEntity entity);
    }
}

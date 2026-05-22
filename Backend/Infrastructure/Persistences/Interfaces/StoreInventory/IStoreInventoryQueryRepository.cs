using Infrastructure.Persistences.ReadModels.StoreInventory;

namespace Infrastructure.Persistences.Interfaces.StoreInventory
{
    public interface IStoreInventoryQueryRepository
    {
        IQueryable<StoreInventoryReadModel> GetInventoryQueryable(int storeId);
        Task<List<KardexMovementReadModel>> GetKardexByProductAsync(int storeId, int productId, DateTime? startDate, DateTime? endDate);
        Task<(List<StoreInventoryListReadModel> Data, int TotalRecords)> GetInventoryListAsync(int storeId, int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
        Task<(List<InventoryPivotReadModel> Data, int TotalRecords)> GetInventoryPivotAsync(int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    }
}

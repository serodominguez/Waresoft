using Infrastructure.Persistences.ReadModels.StoreInventory;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStoreInventoryQueryRepository
    {
        IQueryable<StoreInventoryReadModel> GetInventoryListQueryable(int storeId);
        Task<List<KardexMovementReadModel>> GetKardexByProductAsync(int storeId, int productId, DateTime? startDate, DateTime? endDate);
        Task<(List<InventoryPivotReadModel> Data, int TotalRecords)> GetInventoryPivotAsync(int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
        Task<(List<InventoryCalculatedReadModel> Data, int TotalRecords)> GetInventoryCalculatedAsync(int storeId, int? numberFilter, string? textFilter, bool? stateFilter, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);

    }
}

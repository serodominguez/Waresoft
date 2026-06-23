using Infrastructure.Persistences.ReadModels.Dashboard;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IDashboardQueryRepository
    {
        Task<DashboardGoodsIssueStatsReadModel> GetGoodsIssueStatsLast14DaysAsync(int storeId, CancellationToken cancellationToken);
        Task<DashboardStoreInventoryStatsReadModel> GetInventoryStatsCurrentMonthAsync(int storeId, CancellationToken cancellationToken);
        Task<IEnumerable<DashboardMovementStatsReadModel>> GetMovementsLast6MonthsAsync(int storeId, CancellationToken cancellationToken);
        Task<DashboardProductReplenishmentReadModel> GetProductReplenishmentAsync(int storeId, CancellationToken cancellationToken);
        Task<DashboardProductStatsReadModel> GetProductStatsCurrentMonthAsync(CancellationToken cancellationToken);
        Task<IEnumerable<DashboardTransferByStoreReadModel>> GetTransfersByStoreLast6MonthsAsync(int storeId, CancellationToken cancellationToken);
        Task<DashboardTransferPendingReadModel> GetTransferPendingAsync(int storeId, CancellationToken cancellationToken);
        Task<IEnumerable<DashboardTransferStatusReadModel>> GetTransferStatusLast6MonthsAsync(int storeId, CancellationToken cancellationToken);

    }
}

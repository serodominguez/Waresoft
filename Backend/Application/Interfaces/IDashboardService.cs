using Application.Commons.Bases.Response;
using Application.Dtos.Response.Dashboard;

namespace Application.Interfaces
{
    public interface IDashboardService
    {
        Task<BaseResponse<DashboardGoodsIssueStatsResponseDto>> GetGoodsIssueStats(int authenticatedStoreId, CancellationToken cancellationToken);
        Task<BaseResponse<DashboardStoreInventoryStatsResponseDto>> GetInventoryStats(int authenticatedStoreId, CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<DashboardMovementResponseDto>>> GetMovementsStats(int authenticatedStoreId, CancellationToken cancellationToken);
        Task<BaseResponse<DashboardProductReplenishmentResponseDto>> GetProductReplenishment(int authenticatedStoreId, CancellationToken cancellationToken);
        Task<BaseResponse<DashboardProductStatsResponseDto>> GetProductStats(CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<DashboardTransferByStoreResponseDto>>> GetTransfersByStore(int authenticatedStoreId, CancellationToken cancellationToken);
        Task<BaseResponse<DashboardTransferStatsResponseDto>> GetTransferPending(int authenticatedStoreId, CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<DashboardTransferStatusResponseDto>>> GetTransferStatus(int authenticatedStoreId, CancellationToken cancellation);
    }
}

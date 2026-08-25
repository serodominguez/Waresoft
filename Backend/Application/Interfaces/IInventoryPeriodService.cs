using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.InventoryPeriod;
using Application.Dtos.Response.InventoryPeriod;

namespace Application.Interfaces
{
    public interface IInventoryPeriodService
    {
        Task<BaseResponse<IEnumerable<InventoryPeriodResponseDto>>> ListPeriods(int storeId, BaseFiltersRequest filters);
        Task<BaseResponse<InventoryPeriodDetailResponseDto>> GetPeriodDetail(int periodId);
        Task<BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>>> GetClosingByPeriod(int periodId);
        Task<BaseResponse<IEnumerable<InventoryPeriodOpeningResponseDto>>> GetOpeningByPeriod(int periodId);
        Task<BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>>> GetSystemStockCalculated(int periodId);
        Task<BaseResponse<bool>> OpenPeriod(int authenticatedUserId, int authenticatedStoreId, InventoryPeriodOpenRequestDto requestDto);
        Task<BaseResponse<bool>> ClosePeriod(int authenticatedUserId, int authenticatedStoreId, InventoryPeriodCloseRequestDto requestDto);
    }
}

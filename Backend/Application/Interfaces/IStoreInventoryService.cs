using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;

namespace Application.Interfaces
{
    public interface IStoreInventoryService
    {
        Task<BaseResponse<IEnumerable<StoreInventoryResponseDto>>> ListInventory(int authenticatedStoreId, BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<StoreInventoryCalculatedResponseDto>>> ListInventoryCalculated(int authenticatedStoreId, BaseFiltersRequest filters);
        Task<BaseResponse<StoreInventoryPivotResponseDto>> ListInventoryPivot(BaseFiltersRequest filters);
        Task<BaseResponse<StoreInventoryKardexResponseDto>> ListKardexInventory(int authenticatedStoreId, int productId, BaseFiltersRequest filters);
        Task<BaseResponse<bool>> UpdateMinimumAndPriceByProduct(int authenticatedUserId, int authenticatedStoreId, StoreInventoryRequestDto requestDto);
        
    }
}

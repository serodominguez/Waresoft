using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Store;
using Application.Dtos.Response.Store;

namespace Application.Interfaces
{
    public interface IStoreService
    {
        Task<BaseResponse<IEnumerable<StoreResponseDto>>> ListStores(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<StoreSelectResponseDto>>> ListSelectStores();
        Task<BaseResponse<StoreResponseDto>> StoreById(int storeId);
        Task<BaseResponse<bool>> RegisterStore(int authenticatedUserId, StoreRequestDto requestDto);
        Task<BaseResponse<bool>> EditStore(int authenticatedUserId, int storeId, StoreRequestDto requestDto);
        Task<BaseResponse<bool>> EnableStore(int authenticatedUserId, int storeId);
        Task<BaseResponse<bool>> DisableStore(int authenticatedUserId, int storeId);
        Task<BaseResponse<bool>> RemoveStore(int authenticatedUserId, int storeId);
    }
}

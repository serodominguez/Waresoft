using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Supplier;
using Application.Dtos.Response.Supplier;

namespace Application.Interfaces
{
    public interface ISupplierService
    {
        Task<BaseResponse<IEnumerable<SupplierResponseDto>>> ListSuppliers(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<SupplierSelectResponseDto>>> ListSelectSuppliers();
        Task<BaseResponse<SupplierResponseDto>> SupplierById(int supplierId);
        Task<BaseResponse<bool>> RegisterSupplier(int authenticatedUserId, SupplierRequestDto requestDto);
        Task<BaseResponse<bool>> EditSupplier(int authenticatedUserId, int supplierId, SupplierRequestDto requestDto);
        Task<BaseResponse<bool>> EnableSupplier(int authenticatedUserId, int supplierId);
        Task<BaseResponse<bool>> DisableSupplier(int authenticatedUserId, int supplierId);
        Task<BaseResponse<bool>> RemoveSupplier(int authenticatedUserId, int supplierId);
    }
}

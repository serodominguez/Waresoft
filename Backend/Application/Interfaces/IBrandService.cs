using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Brand;
using Application.Dtos.Response.Brand;

namespace Application.Interfaces
{
    public interface IBrandService
    {
        Task<BaseResponse<IEnumerable<BrandResponseDto>>> ListBrands(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<BrandSelectResponseDto>>> ListSelectBrands();
        Task<BaseResponse<BrandResponseDto>> BrandById(int brandId);
        Task<BaseResponse<bool>> RegisterBrand(int authenticatedUserId, BrandRequestDto requestDto);
        Task<BaseResponse<bool>> EditBrand(int authenticatedUserId, int brandId, BrandRequestDto requestDto);
        Task<BaseResponse<bool>> EnableBrand(int authenticatedUserId, int brandId);
        Task<BaseResponse<bool>> DisableBrand(int authenticatedUserId, int brandId);
        Task<BaseResponse<bool>> RemoveBrand(int authenticatedUserId, int brandId);
    }
}

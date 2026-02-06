using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Product;
using Application.Dtos.Response.Product;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<BaseResponse<IEnumerable<ProductResponseDto>>> ListProducts(BaseFiltersRequest filters);
        Task<BaseResponse<ProductResponseDto>> ProductById(int productId);
        Task<BaseResponse<bool>> RegisterProduct(int authenticatedUserId, ProductRequestDto requestDto);
        Task<BaseResponse<bool>> EditProduct(int authenticatedUserId, int productId, ProductRequestDto requestDto);
        Task<BaseResponse<bool>> EnableProduct(int authenticatedUserId, int productId);
        Task<BaseResponse<bool>> DisableProduct(int authenticatedUserId, int productId);
        Task<BaseResponse<bool>> RemoveProduct(int authenticatedUserId, int productId);
    }
}

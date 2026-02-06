using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Category;
using Application.Dtos.Response.Category;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<BaseResponse<IEnumerable<CategoryResponseDto>>> ListCategories(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<CategorySelectResponseDto>>> ListSelectCategories();
        Task<BaseResponse<CategoryResponseDto>> CategoryById(int categoryId);
        Task<BaseResponse<bool>> RegisterCategory(int authenticatedUserId, CategoryRequestDto requestDto);
        Task<BaseResponse<bool>> EditCategory(int authenticatedUserId, int categoryId, CategoryRequestDto requestDto);
        Task<BaseResponse<bool>> EnableCategory(int authenticatedUserId, int categoryId);
        Task<BaseResponse<bool>> DisableCategory(int authenticatedUserId, int categoryId);
        Task<BaseResponse<bool>> RemoveCategory(int authenticatedUserId, int categoryId);
    }
}

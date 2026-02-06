using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Module;
using Application.Dtos.Response.Module;

namespace Application.Interfaces
{
    public interface IModuleService
    {
        Task<BaseResponse<IEnumerable<ModuleResponseDto>>> ListModules(BaseFiltersRequest filters);
        Task<BaseResponse<ModuleResponseDto>> ModuleById(int moduleId);
        Task<BaseResponse<bool>> RegisterModule(int authenticatedUserId, ModuleRequestDto requestDto);
        Task<BaseResponse<bool>> EditModule(int authenticatedUserId, int moduleId, ModuleRequestDto requestDto);
        Task<BaseResponse<bool>> EnableModule(int authenticatedUserId, int moduleId);
        Task<BaseResponse<bool>> DisableModule(int authenticatedUserId, int moduleId);
        Task<BaseResponse<bool>> RemoveModule(int authenticatedUserId, int moduleId);
    }
}

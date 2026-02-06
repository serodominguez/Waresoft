using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Role;
using Application.Dtos.Response.Role;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<BaseResponse<IEnumerable<RoleResponseDto>>> ListRoles(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<RoleSelectResponseDto>>> ListSelectRoles();
        Task<BaseResponse<RoleResponseDto>> RoleById(int roleId);
        Task<BaseResponse<bool>> RegisterRole(int authenticatedUserId, RoleRequestDto requestDto);
        Task<BaseResponse<bool>> EditRole(int authenticatedUserId, int roleId, RoleRequestDto requestDto);
        Task<BaseResponse<bool>> EnableRole(int authenticatedUserId, int roleId);
        Task<BaseResponse<bool>> DisableRole(int authenticatedUserId, int roleId);
        Task<BaseResponse<bool>> RemoveRole(int authenticatedUserId, int roleId);
    }
}

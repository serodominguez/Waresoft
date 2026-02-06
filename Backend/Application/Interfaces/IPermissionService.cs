using Application.Commons.Bases.Response;
using Application.Dtos.Request.Permission;
using Application.Dtos.Response.Permission;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> UserPermissions(int userId, string moduleName, string actionName);
        Task<BaseResponse<bool>> UpdatePermissions(int authenticatedUserId, List<PermissionRequestDto> permissions);
        Task<BaseResponse<IEnumerable<PermissionByUserResponseDto>>> ListUserPermissions(int userId);
        Task<BaseResponse<IEnumerable<PermissionByRoleResponseDto>>> PermissionsByRole(int roleId);
    }
}

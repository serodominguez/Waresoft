using Application.Commons.Bases.Response;
using Application.Dtos.Request.Permission;
using Application.Dtos.Response.Permission;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UserPermissions(int userId, string moduleName, string actionName)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);

            if (user == null || !user.Status) return false;

            return await _unitOfWork.Permission.GetPermissionsAsync(user.IdRole, moduleName, actionName);
        }

        public async Task<BaseResponse<bool>> UpdatePermissions(int authenticatedUserId, List<PermissionRequestDto> permissionsDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                if (permissionsDto == null || !permissionsDto.Any())
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var existingPermissions = await _unitOfWork.Permission
                    .GetByIdsAsync(permissionsDto.Select(p => p.IdPermission).ToList());

                var permissionsDict = existingPermissions.ToDictionary(p => p.Id);
                var listPermissionsUpdate = new List<PermissionEntity>();

                foreach (var permission in permissionsDto)
                {
                    if (!permissionsDict.TryGetValue(permission.IdPermission, out var existing))
                    {
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                        return response;
                    }

                    if (permission.Status != existing.Status)
                    {
                        var permissionToUpdate = PermissionMap.PermissionsMapping(permission);
                        permissionToUpdate.AuditUpdateUser = authenticatedUserId;
                        permissionToUpdate.AuditUpdateDate = DateTime.Now;
                        listPermissionsUpdate.Add(permissionToUpdate);
                    }
                }

                if (!listPermissionsUpdate.Any())
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                var result = await _unitOfWork.Permission.UpdatePermissionsRangeAsync(listPermissionsUpdate);

                if (result)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<PermissionByUserResponseDto>>> ListUserPermissions(int userId)
        {
            var response = new BaseResponse<IEnumerable<PermissionByUserResponseDto>>();
            try
            {
                var user = await _unitOfWork.User.GetByIdAsync(userId);

                if (user == null || !user.Status)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_NOT_FOUND;
                    return response;
                }

                var permissions = await _unitOfWork.Permission.PermissionsByRoleAsync(user.IdRole);

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                                .Where(p => p.Status && p.Module!.Status && p.Action!.Status)
                                .Select(PermissionMap.PermissionsByUserResponseDtoMapping);

                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<IEnumerable<PermissionByRoleResponseDto>>> PermissionsByRole(int roleId)
        {
            var response = new BaseResponse<IEnumerable<PermissionByRoleResponseDto>>();
            try
            {
                var role = await _unitOfWork.Role.GetByIdAsync(roleId);
                if (role == null || !role.Status)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_NOT_FOUND;
                    return response;
                }

                var permissions = await _unitOfWork.Permission.PermissionsByRoleAsync(roleId);

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                                .Where(p => p.Module!.Status && p.Action!.Status)
                                .Select(PermissionMap.PermissionsByRoleResponseDtoMapping);

                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }
    }
}

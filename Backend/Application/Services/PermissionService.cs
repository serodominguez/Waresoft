using Application.Commons.Bases.Response;
using Application.Dtos.Request.Permission;
using Application.Dtos.Response.Permission;
using Application.Interfaces;
using Application.Mappers;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            var user = await _unitOfWork.UserQuery.GetUserByIdQueryable(userId)
                .FirstOrDefaultAsync();

            if (user == null || !user.Status) return false;

            return await _unitOfWork.PermissionQuery.GetPermissionsAsync(user.IdRole, moduleName, actionName);
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

                var existingPermissions = await _unitOfWork.PermissionCommand
                    .GetListOfIdsAsQueryable(permissionsDto.Select(p => p.IdPermission).ToList())
                    .ToListAsync();

                var permissionsDict = existingPermissions.ToDictionary(p => p.Id);
                var hasChanges = false;

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
                        existing.Status = permission.Status;
                        existing.AuditUpdateUser = authenticatedUserId;
                        existing.AuditUpdateDate = DateTime.Now;
                        hasChanges = true;
                    }
                }

                if (!hasChanges)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
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
                var user = await _unitOfWork.UserQuery.GetUserByIdQueryable(userId)
                    .FirstOrDefaultAsync();

                if (user == null || !user.Status)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_NOT_FOUND;
                    return response;
                }

                var permissions = await _unitOfWork.PermissionQuery.PermissionsByRoleAsQueryable(user.IdRole)
                    .ToListAsync();

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                                .Where(p => p.Status && p.StatusModule && p.StatusAction)
                                .Select(PermissionMapp.PermissionsByUserResponseDtoMapping);

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
                var role = await _unitOfWork.RoleQuery.GetRoleByIdQueryable(roleId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (role == null || !role.Status)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_USER_NOT_FOUND;
                    return response;
                }

                var permissions = await _unitOfWork.PermissionQuery.PermissionsByRoleAsQueryable(role.Id)
                    .ToListAsync();

                if (permissions != null && permissions.Any())
                {
                    response.Data = permissions
                                .Where(p => p.StatusModule && p.StatusAction)
                                .Select(PermissionMapp.PermissionsByRoleResponseDtoMapping);

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

using Application.Dtos.Request.Permission;
using Application.Dtos.Response.Permission;
using Domain.Entities;

namespace Application.Mappers
{
    public static class PermissionMap
    {
        public static PermissionEntity PermissionsMapping(PermissionRequestDto dto)
        {
            return new PermissionEntity
            {
                Id = dto.IdPermission,
                Status = dto.Status
            };
        }

        public static PermissionByUserResponseDto PermissionsByUserResponseDtoMapping(PermissionEntity entity)
        {
            return new PermissionByUserResponseDto
            {
                Action = entity.Action.ActionName!,
                Module = entity.Module.ModuleName!
            };
        }

        public static PermissionByRoleResponseDto PermissionsByRoleResponseDtoMapping(PermissionEntity entity)
        {
            return new PermissionByRoleResponseDto
            {
                IdPermission = entity.Id,
                IdRole = entity.IdRole,
                IdModule = entity.IdModule,
                ModuleName = entity.Module?.ModuleName,
                IdAction = entity.IdAction,
                ActionName = entity.Action?.ActionName,
                Status = entity.Status
            };
        }
    }
}

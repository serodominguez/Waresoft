using Application.Dtos.Response.Permission;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Permission;

namespace Application.Mappers
{
    public static class PermissionMapp
    {
        public static PermissionByUserResponseDto PermissionsByUserResponseDtoMapping(PermissionReadModel model)
        {
            return new PermissionByUserResponseDto
            {
                Action = model.ActionName!,
                Module = model.ModuleName!
            };
        }

        public static PermissionByRoleResponseDto PermissionsByRoleResponseDtoMapping(PermissionReadModel model)
        {
            return new PermissionByRoleResponseDto
            {
                IdPermission = model.Id,
                IdRole = model.IdRole,
                IdModule = model.IdModule,
                ModuleName = model.ModuleName,
                IdAction = model.IdAction,
                ActionName = model.ActionName,
                Status = model.IsActive
            };
        }
    }
}

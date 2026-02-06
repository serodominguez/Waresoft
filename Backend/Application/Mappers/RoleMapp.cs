using Application.Dtos.Request.Role;
using Application.Dtos.Response.Role;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class RoleMapp
    {
        public static RoleEntity RolesMapping(RoleRequestDto dto)
        {
            return new RoleEntity
            {
                RoleName = dto.RoleName.NormalizeString()
            };
        }

        public static RoleResponseDto RolesResponseDtoMapping(RoleEntity entity)
        {
            return new RoleResponseDto
            {
                IdRole = entity.Id,
                RoleName = entity.RoleName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusRole = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static RoleSelectResponseDto RolesSelectResponseDtoMapping(RoleEntity entity)
        {
            return new RoleSelectResponseDto
            {
                IdRole = entity.Id,
                RoleName = entity.RoleName.ToTitleCase()
            };
        }
    }
}

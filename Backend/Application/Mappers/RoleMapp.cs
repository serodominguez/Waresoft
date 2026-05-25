using Application.Dtos.Request.Role;
using Application.Dtos.Response.Role;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Role;
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

        public static RoleResponseDto RolesResponseDtoMapping(RoleReadModel model)
        {
            return new RoleResponseDto
            {
                IdRole = model.Id,
                RoleName = model.RoleName.ToSentenceCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusRole = ((States)(model.IsActive ? 1 : 0)).ToString()
            };
        }

        public static RoleSelectResponseDto RolesSelectResponseDtoMapping(RoleSelectReadModel model)
        {
            return new RoleSelectResponseDto
            {
                IdRole = model.Id,
                RoleName = model.RoleName.ToSentenceCase()
            };
        }
    }
}

using Application.Dtos.Request.Module;
using Application.Dtos.Response.Module;
using Domain.Entities;
using Utilities.Static;

namespace Application.Mappers
{
    public static class ModuleMapp
    {
        public static ModuleEntity ModulesMapping(ModuleRequestDto dto)
        {
            return new ModuleEntity
            {
                ModuleName = dto.ModuleName
            };
        }

        public static ModuleResponseDto ModulesResponseDtoMapping(ModuleEntity entity)
        {
            return new ModuleResponseDto
            {
                IdModule = entity.Id,
                ModuleName = entity.ModuleName,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusModule = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };
        }
    }
}

using Application.Dtos.Request.Module;
using Application.Dtos.Response.Module;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Module;
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

        public static ModuleResponseDto ModulesResponseDtoMapping(ModuleReadModel model)
        {
            return new ModuleResponseDto
            {
                IdModule = model.Id,
                ModuleName = model.ModuleName,
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusModule = ((States)(model.Status ? 1 : 0)).ToString()
            };
        }
    }
}

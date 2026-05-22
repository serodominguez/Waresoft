using Application.Dtos.Request.Store;
using Application.Dtos.Response.Store;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Store;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class StoreMapp
    {
        public static StoreEntity StoresMapping(StoreRequestDto dto)
        {
            return new StoreEntity
            {
                StoreName = dto.StoreName.NormalizeString(),
                Manager = dto.Manager.NormalizeString(),
                Address = dto.Address.NormalizeString(),
                PhoneNumber = dto.PhoneNumber,
                City = dto.City.NormalizeString(),
                Email = dto.Email,
                ProfitMargin = dto.ProfitMargin,
                Type = dto.Type.NormalizeString()
            };
        }

        public static StoreResponseDto StoresResponseDtoMapping(StoreReadModel model)
        {
            return new StoreResponseDto
            {
                IdStore = model.Id,
                StoreName = model.StoreName.ToTitleCase(),
                Manager = model.Manager.ToSentenceCase(),
                Address = model.Address.ToSentenceCaseMultiple(),
                PhoneNumber = model.PhoneNumber,
                City = model.City.ToTitleCase(),
                Email = model.Email,
                ProfitMargin= model.ProfitMargin,
                Type = model.Type.ToSentenceCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusStore = ((States)(model.Status ? 1 : 0)).ToString()
            };
        }

        public static StoreSelectResponseDto StoresSelectResponseDtoMapping(StoreSelectReadModel model)
        {
            return new StoreSelectResponseDto
            {
                IdStore = model.Id,
                StoreName = model.StoreName.ToTitleCase()
            };
        }
    }
}

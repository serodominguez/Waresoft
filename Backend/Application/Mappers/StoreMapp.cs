using Application.Dtos.Request.Store;
using Application.Dtos.Response.Store;
using Domain.Entities;
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
                Email = dto.Email.NormalizeString(),
                Type = dto.Type.NormalizeString()
            };
        }

        public static StoreResponseDto StoresResponseDtoMapping(StoreEntity entity)
        {
            return new StoreResponseDto
            {
                IdStore = entity.Id,
                StoreName = entity.StoreName.ToTitleCase(),
                Manager = entity.Manager.ToTitleCase(),
                Address = entity.Address.ToTitleCase(),
                PhoneNumber = entity.PhoneNumber,
                City = entity.City.ToTitleCase(),
                Email = entity.Email?.ToLower(),
                Type = entity.Type.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusStore = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static StoreSelectResponseDto StoresSelectResponseDtoMapping(StoreEntity entity)
        {
            return new StoreSelectResponseDto
            {
                IdStore = entity.Id,
                StoreName = entity.StoreName.ToTitleCase()
            };
        }
    }
}

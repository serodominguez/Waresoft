using Application.Dtos.Request.Brand;
using Application.Dtos.Response.Brand;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class BrandMapp
    {
        public static BrandEntity BrandsMapping(BrandRequestDto dto)
        {
            return new BrandEntity
            {
                BrandName = dto.BrandName.NormalizeString()
            };
        }

        public static BrandResponseDto BrandsResponseDtoMapping(BrandEntity entity)
        {
            return new BrandResponseDto
            {
                IdBrand = entity.Id,
                BrandName = entity.BrandName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusBrand = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static BrandSelectResponseDto BrandsSelectResponseDtoMapping(BrandEntity entity)
        {
            return new BrandSelectResponseDto
            {
                IdBrand = entity.Id,
                BrandName = entity.BrandName.ToTitleCase()
            };
        }
    }
}

using Application.Dtos.Request.Brand;
using Application.Dtos.Response.Brand;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Brand;
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

        public static BrandResponseDto BrandsResponseDtoMapping(BrandReadModel model)
        {
            return new BrandResponseDto
            {
                IdBrand = model.Id,
                BrandName = model.BrandName.ToTitleCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusBrand = ((States)(model.Status ? 1 : 0)).ToString()
            };
        }

        public static BrandSelectResponseDto BrandsSelectResponseDtoMapping(BrandSelectReadModel model)
        {
            return new BrandSelectResponseDto
            {
                IdBrand = model.Id,
                BrandName = model.BrandName.ToTitleCase()
            };
        }
    }
}

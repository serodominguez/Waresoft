using Application.Dtos.Request.Product;
using Application.Dtos.Response.Product;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class ProductMapp
    {
        public static ProductEntity ProductsMapping(ProductRequestDto dto)
        {
            return new ProductEntity
            {
                Code = dto.Code.NormalizeString(),
                Description = dto.Description.NormalizeString(),
                Material = dto.Material.NormalizeString(),
                Color = dto.Color.NormalizeString(),
                UnitMeasure = dto.UnitMeasure.NormalizeString(),
                IdBrand = dto.IdBrand,
                IdCategory = dto.IdCategory
            };
        }

        public static ProductResponseDto ProductsResponseDtoMapping(ProductEntity entity)
        {
            return new ProductResponseDto
            {
                IdProduct = entity.Id,
                Code = entity.Code.ToTitleCase(),
                Description = entity.Description.ToTitleCase(),
                Material = entity.Material.ToTitleCase(),
                Color = entity.Color.ToTitleCase(),
                UnitMeasure = entity.UnitMeasure.ToTitleCase(),
                IdBrand = entity.IdBrand,
                BrandName = entity.Brand?.BrandName.ToTitleCase(),
                IdCategory = entity.IdCategory,
                CategoryName = entity.Category?.CategoryName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusProduct = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };
        }
    }
}

using Application.Dtos.Request.Product;
using Application.Dtos.Response.Product;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Product;
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

        public static ProductResponseDto ProductsResponseDtoMapping(ProductReadModel model)
        {
            return new ProductResponseDto
            {
                IdProduct = model.Id,
                Code = model.Code,
                Description = model.Description.ToSentenceCase(),
                Material = model.Material.ToSentenceCase(),
                Color = model.Color.ToSentenceCase(),
                UnitMeasure = model.UnitMeasure.ToSentenceCase(),
                Image = model.Image,
                IdBrand = model.IdBrand,
                BrandName = model.BrandName.ToSentenceCase(),
                IdCategory = model.IdCategory,
                CategoryName = model.CategoryName.ToSentenceCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusProduct = ((States)(model.Status ? 1 : 0)).ToString()
            };
        }
    }
}

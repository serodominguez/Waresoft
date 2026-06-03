using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Product;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class ProductProjection
    {
        public static Expression<Func<ProductEntity, ProductReadModel>> ToSummary =>
            p => new ProductReadModel
            {
                Id = p.Id,
                Code = p.Code,
                Description = p.Description,
                Material = p.Material,
                Color = p.Color,
                UnitMeasure = p.UnitMeasure,
                Image = p.Image,
                IdBrand = p.IdBrand,
                BrandName = p.Brand.BrandName,
                IdCategory = p.IdCategory,
                CategoryName = p.Category.CategoryName,
                AuditCreateDate = p.AuditCreateDate,
                IsActive = p.IsActive
            };

        public static Expression<Func<ProductEntity, ProductSelectReadModel>> ToSelect =>
            p => new ProductSelectReadModel
            {
                Id = p.Id,
                Code = p.Code,
                Description = p.Description,
                IsActive = p.IsActive
            };
    }
}

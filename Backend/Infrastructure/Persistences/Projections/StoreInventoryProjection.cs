using Domain.Entities;
using Infrastructure.Persistences.ReadModels.StoreInventory;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class StoreInventoryProjection
    {
        public static Expression<Func<StoreInventoryEntity, StoreInventoryReadModel>> ToSummary =>
            i => new StoreInventoryReadModel
            {
                IdStore = i.IdStore,
                IdProduct = i.IdProduct,
                StockAvailable = i.StockAvailable,
                StockInTransit = i.StockInTransit,
                Price = i.Price,
                StoreName = i.Store.StoreName,
                Replenishment = i.Product.Replenishment,
                Code = i.Product.Code,
                Description = i.Product.Description,
                Material = i.Product.Material,
                Color = i.Product.Color,
                UnitMeasure = i.Product.UnitMeasure,
                BrandName = i.Product.Brand.BrandName,
                CategoryName = i.Product.Category.CategoryName,
                Status = i.Product.Status,
                AuditCreateDate = i.Product.AuditCreateDate
            };
    }
}

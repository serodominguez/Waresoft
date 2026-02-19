using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class StoreInventoryMapp
    {
        public static StoreInventoryEntity StoreInventoryMapping (StoreInventoryRequestDto dto)
        {
            return new StoreInventoryEntity
            {
                IdProduct = dto.IdProduct,
                Price = dto.Price
            };
        }

        public static StoreInventoryResponseDto StoreInventoryMapping(StoreInventoryEntity entity)
        {
            return new StoreInventoryResponseDto
            {
                IdStore = entity.IdStore,
                IdProduct = entity.IdProduct,
                StockAvailable = entity.StockAvailable,
                StockInTransit = entity.StockInTransit,
                Price = entity.Price,
                Replenishment = ((Replenishment)(entity.Product.Replenishment)).ToString().ReplaceUnderscoresWithSpace(),
                Code = entity.Product?.Code,
                Description = entity.Product?.Description.ToSentenceCase(),
                Material = entity.Product?.Material.ToSentenceCase(),
                Color = entity.Product?.Color.ToSentenceCase(),
                UnitMeasure = entity.Product?.UnitMeasure.ToSentenceCase(),
                BrandName = entity.Product?.Brand?.BrandName.ToSentenceCase(),
                CategoryName = entity.Product?.Category?.CategoryName.ToSentenceCase(),
                AuditCreateDate = entity.Product?.AuditCreateDate?.ToString("dd/MM/yyyy HH:mm"),
            };
        }

        public static StoreInventoryPivotResponseDto StoreInventoryPivotMapping(List<StoreInventoryEntity> inventory, List<StoreEntity> entity)
        {
            var stores = entity
                .Where(s => !string.IsNullOrEmpty(s.StoreName))
                .Select(s => s.StoreName!)
                .Distinct()
                .ToList();

            var rows = inventory
                .GroupBy(i => i.Product.Id)
                .Select(g => new StoreInventoryPivotRowResponseDto
                {
                    Code = g.First().Product.Code,
                    Color = g.First().Product.Color.ToSentenceCase(),
                    BrandName = g.First().Product.Brand.BrandName.ToSentenceCase(),
                    CategoryName = g.First().Product.Category.CategoryName.ToSentenceCase(),
                    AuditCreateDate = g.First().Product.AuditCreateDate?.ToString("dd/MM/yyyy HH:mm"),
                    StockByStore = stores.ToDictionary(
                        store => store.ToSentenceCase() ?? store,
                        store => g.FirstOrDefault(i => i.Store.StoreName == store)?.StockAvailable ?? 0
                    )
                }).ToList();

            return new StoreInventoryPivotResponseDto { Stores = stores, Rows = rows };
        }
    }
}

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
                Availability = ((Availability)(entity.Product.Availability)).ToString().ReplaceUnderscoresWithSpace(),
                Code = entity.Product?.Code,
                Description = entity.Product?.Description.ToSentenceCase(),
                Material = entity.Product?.Material.ToSentenceCase(),
                Color = entity.Product?.Color.ToSentenceCase(),
                UnitMeasure = entity.Product?.UnitMeasure.ToSentenceCase(),
                BrandName = entity.Product?.Brand?.BrandName.ToSentenceCase(),
                CategoryName = entity.Product?.Category?.CategoryName.ToSentenceCase()
            };
        }
    }
}

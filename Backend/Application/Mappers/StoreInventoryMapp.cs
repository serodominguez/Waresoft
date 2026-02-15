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
                Description = entity.Product?.Description.ToTitleCase(),
                Material = entity.Product?.Material.ToTitleCase(),
                Color = entity.Product?.Color.ToTitleCase(),
                UnitMeasure = entity.Product?.UnitMeasure.ToTitleCase(),
                BrandName = entity.Product?.Brand?.BrandName.ToTitleCase(),
                CategoryName = entity.Product?.Category?.CategoryName.ToTitleCase()
            };
        }
    }
}

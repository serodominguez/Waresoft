using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Store;
using Infrastructure.Persistences.ReadModels.StoreInventory;
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

        public static StoreInventoryResponseDto StoreInventoryResponseMapping(StoreInventoryReadModel model)
        {
            return new StoreInventoryResponseDto
            {
                IdStore = model.IdStore,
                IdProduct = model.IdProduct,
                StockAvailable = model.StockAvailable,
                StockInTransit = model.StockInTransit,
                Price = model.Price,
                Replenishment = ((Replenishment)(model.Replenishment!)).ToString().ReplaceUnderscoresWithSpace(),
                Code = model.Code,
                Description = model.Description?.ToSentenceCase(),
                Material = model.Material?.ToSentenceCase(),
                Color = model.Color?.ToSentenceCase(),
                UnitMeasure = model.UnitMeasure?.ToSentenceCase(),
                BrandName = model.BrandName?.ToSentenceCase(),
                CategoryName = model.CategoryName?.ToSentenceCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null
            };
        }

        public static StoreInventoryCalculatedResponseDto StoreInventoryCalculatedMapping(InventoryCalculatedReadModel model)
        {
            return new StoreInventoryCalculatedResponseDto
            {
                IdStore = model.IdStore,
                IdProduct = model.IdProduct,
                StockAvailable = model.StockAvailable,
                CalculatedStock = model.CalculatedStock,
                StockDifference = model.StockAvailable - model.CalculatedStock,
                StockInTransit = model.StockInTransit,
                MinimumStock = model.MinimumStock,
                Price = model.Price,
                Replenishment = ((Replenishment)(model.Replenishment!)).ToString().ReplaceUnderscoresWithSpace(),
                Code = model.Code,
                Description = model.Description?.ToSentenceCase(),
                Material = model.Material?.ToSentenceCase(),
                Color = model.Color?.ToSentenceCase(),
                UnitMeasure = model.UnitMeasure?.ToSentenceCase(),
                BrandName = model.BrandName?.ToSentenceCase(),
                CategoryName = model.CategoryName?.ToSentenceCase(),
                AuditCreateDate = model.AuditCreateDate.ToString("dd/MM/yyyy HH:mm")
            };
        }

        public static StoreInventoryPivotResponseDto StoreInventoryPivotMapping(List<InventoryPivotReadModel> items, List<StoreReadModel> stores)
        {
            var storeDict = stores
                .Where(s => !string.IsNullOrEmpty(s.StoreName))
                .ToDictionary(s => s.Id, s => s.StoreName!.ToSentenceCase()!);

            var rows = items.Select(item => new StoreInventoryPivotRowResponseDto
            {
                Image = item.Image,
                Code = item.Code,
                Description = item.Description?.ToSentenceCase(),
                Material = item.Material?.ToSentenceCase(),
                Color = item.Color?.ToSentenceCase(),
                BrandName = item.BrandName?.ToSentenceCase(),
                CategoryName = item.CategoryName?.ToSentenceCase(),
                AuditCreateDate = item.AuditCreateDate.ToString("dd/MM/yyyy HH:mm"),
                StockByStore = ParseStoreStocks(item.StoreStocks, storeDict)
            }).ToList();

            return new StoreInventoryPivotResponseDto
            {
                Stores = storeDict.Values.ToList(),
                Rows = rows
            };
        }

        public static StoreInventoryKardexResponseDto StoreInventoryKardexMapping(StoreInventoryReadModel product, List<KardexMovementReadModel> movements, int calculateStock, int stockDifference)
        {
            return new StoreInventoryKardexResponseDto
            {
                IdProduct = product.IdProduct,
                Code = product.Code,
                Description = product.Description?.ToSentenceCase(),
                Material = product.Material?.ToSentenceCase(),
                Color = product.Color?.ToSentenceCase(),
                UnitMeasure = product.UnitMeasure?.ToSentenceCase(),
                CurrentStock = product.StockAvailable,
                CalculatedStock = calculateStock,
                StockDifference = stockDifference,
                Movements = movements.Select(m => new StoreInventoryKardexMovementDto
                {
                    IdProduct = m.IdProduct,
                    Quantity = m.Quantity,
                    IdMovement = m.IdMovement,
                    Code = m.Code,
                    Date = m.Date.ToString("dd/MM/yyyy HH:mm"),
                    MovementType = m.MovementType,
                    Type = m.Type.ToSentenceCase(),
                    State = m.MovementType switch
                    {
                        "Traspaso" => m.State.HasValue
                            ? ((Transfers)m.State.Value).ToString()
                            : string.Empty,

                        _ => m.State.HasValue
                            ? ((Movements)m.State.Value).ToString()
                            : string.Empty
                    },
                    AccumulatedStock = m.AccumulatedStock
                }).ToList()
            };
        }

        private static Dictionary<string, int> ParseStoreStocks(string? storeStocks, Dictionary<int, string> storeDict)
        {
            var result = new Dictionary<string, int>();
            if (string.IsNullOrEmpty(storeStocks)) return result;

            var entries = storeStocks.Split(',');
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int stock))
                {
                    if (storeDict.TryGetValue(id, out var name))
                    {
                        result[name] = stock;
                    }
                }
            }
            return result;
        }
    }
}
